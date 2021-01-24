using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{
    public class WebSocketMessage
    {
        public string From { get; set; }
        public string MessageType { get; set; }
        public object Message { get; set; }
    }
}
