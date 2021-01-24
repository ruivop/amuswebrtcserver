using com.ruivop.webrtc.Helpers;
using com.ruivop.webrtc.Messages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Services
{
    public interface IWebsocketReceiver
    {
        Task Connect(HttpContext context, WebSocket webSocket);
    }

    public class ConnectedUser
    {
        public string Id { get; }
        public string Username { get; private set; }
        public WebSocket WebSocket { get; }
        public UsersPool UserConnectionPool { get; set; }

        public bool HasVideo { get; set; }
        public bool HasSound { get; set; }


        public ConnectedUser(WebSocket webSocket)
        {
            Id = Guid.NewGuid().ToString();
            WebSocket = webSocket;
            Username = "";
            HasVideo = false;
            HasSound = false;
        }

        public void setUersPool(UsersPool userConnectionPool)
        {
            if (UserConnectionPool != null)
            {
                UserConnectionPool.RemoveUser(this);

            }

            UserConnectionPool = userConnectionPool;
            if (UserConnectionPool != null)
                UserConnectionPool.AddUser(this);
        }

        public async Task Listen()
        {
            var buffer = new byte[Globals.MAX_USER_MESSAGE_LENGTH];
            try
            {
                WebSocketReceiveResult result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    WebSocketMessage webscketMessage = JsonSerializer.Deserialize<WebSocketMessage>(receivedText);
                    await handleIncomingMessage(webscketMessage);

                    result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro: " + e.Message);
            }


            setUersPool(null);
        }

        private async Task handleIncomingMessage(WebSocketMessage webscketMessage)
        {
            switch (webscketMessage.MessageType)
            {
                case "OfferMessage":
                    webscketMessage.From = this.Username;
                    UserConnectionPool.sendToAllOthers(this, webscketMessage);
                    break;
                case "TextMessage":
                    webscketMessage.From = this.Username;
                    UserConnectionPool.sendToAllOthers(this, webscketMessage);
                    break;
                case "UserStatusChangedMessage":
                    webscketMessage.From = this.Username;
                    var statusMessage = Utils.JsonElementToObject<UserStatusChangedMessage>(webscketMessage.Message);
                    statusMessage.Status = UserChangedStatus.online;
                    HasSound = statusMessage.HasSound;
                    HasVideo = statusMessage.HasVideo;
                    webscketMessage.Message = statusMessage;
                    UserConnectionPool.sendToAllOthers(this, webscketMessage);
                    break;
                case "MultimediaConnectionIceCandidateMessage":
                    webscketMessage.From = this.Username;
                    var usernameTo2 = Utils.JsonElementToObject<MultimediaConnectionIceCandidateMessage>(webscketMessage.Message).UsernameTo;
                    await UserConnectionPool.sendToOnlyAsync(usernameTo2, webscketMessage);
                    break;
                case "MultimediaConnectionRequestMessage":
                    webscketMessage.From = this.Username;
                    var usernameTo = Utils.JsonElementToObject<MultimediaConnectionRequestMessage>(webscketMessage.Message).UsernameTo;
                    await UserConnectionPool.sendToOnlyAsync(usernameTo, webscketMessage);
                    break;
                case "MultimediaConnectionResponseMessage":
                    webscketMessage.From = this.Username;
                    string usernameTo1 = Utils.JsonElementToObject<MultimediaConnectionResponseMessage>(webscketMessage.Message).UsernameTo;
                    await UserConnectionPool.sendToOnlyAsync(usernameTo1, webscketMessage);
                    break;
                case "MultimediaOfferMessage":
                    webscketMessage.From = this.Username;
                    string usernameT = Utils.JsonElementToObject<MultimediaOfferMessage>(webscketMessage.Message).UsernameTo;
                    await UserConnectionPool.sendToOnlyAsync(usernameT, webscketMessage);
                    break;
                case "IdentificationMessage":
                    this.Username = Utils.JsonElementToObject<IdentificationMessage>(webscketMessage.Message).Username;
                    UserConnectionPool.sendToAllOthers(this, new WebSocketMessage
                    {
                        From = Username,
                        MessageType = "UserStatusChangedMessage",
                        Message = new UserStatusChangedMessage
                        {
                            Status = UserChangedStatus.online,
                            HasSound = HasSound,
                            HasVideo = HasVideo
                        }
                    });

                    break;
                default:
                    break;
            }
        }

        public async Task SendMessage(WebSocketMessage toSend)
        {
            await WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(toSend, Globals.JsonOptions))), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public class UsersPool
    {
        public List<ConnectedUser> Users { get; }
        public List<ConnectedUser> Messages { get; }

        public UsersPool()
        {
            Users = new List<ConnectedUser>();
        }

        public void AddUser(ConnectedUser connectedUser)
        {
            Users.Add(connectedUser);
            if (!string.IsNullOrWhiteSpace(connectedUser.Username))
            {
                sendToAllOthers(connectedUser, new WebSocketMessage
                {
                    From = connectedUser.Username,
                    MessageType = "UserStatusChangedMessage",
                    Message = new UserStatusChangedMessage
                    {
                        Status = UserChangedStatus.online,
                        HasSound = connectedUser.HasSound,
                        HasVideo = connectedUser.HasVideo
                    }
                });
            }
        }

        public void RemoveUser(ConnectedUser connectedUser)
        {
            Users.Remove(connectedUser);

            if (!string.IsNullOrWhiteSpace(connectedUser.Username))
            {
                sendToAllOthers(connectedUser, new WebSocketMessage
                {
                    From = connectedUser.Username,
                    MessageType = "UserStatusChangedMessage",
                    Message = new UserStatusChangedMessage
                    {
                        Status = UserChangedStatus.offline,
                        HasSound = false,
                        HasVideo = false
                    }
                });
            }
        }

        public void sendToAllOthers(ConnectedUser exception, WebSocketMessage message)
        {
            Users
                .Where(u => u != exception && !string.IsNullOrWhiteSpace(u.Username))
                .ToList()
                .ForEach(async u =>
                {
                    try
                    {
                        await u.SendMessage(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erro: não foi possível enviar com o erro: " + e.Message);
                    }

                });
        }

        internal async Task sendToOnlyAsync(string usernameTo, WebSocketMessage webscketMessage)
        {
            var userToSend = Users
            .Where(u => u.Username == usernameTo && !string.IsNullOrWhiteSpace(u.Username))
            .FirstOrDefault();
            if (userToSend == null)
                return;

            try
            {
                await userToSend.SendMessage(webscketMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro: não foi possível enviar com o erro: " + e.Message);
            };
        }
    }

    public class WebsocketReceiver : IWebsocketReceiver
    {
        public static UsersPool AllUsersPool = new UsersPool();

        public WebsocketReceiver()
        {
        }

        public async Task Connect(HttpContext context, WebSocket webSocket)
        {
            var User = new ConnectedUser(webSocket);
            User.setUersPool(AllUsersPool);

            await User.Listen();
        }
    }
}
