using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{

    public class InitialUserPool
    {
        public List<InitialUser> OnlineUsers { get; set; }
    }

    public class InitialUser
    {
        public string Username { get; set; }
        public string Color { get; set; }
        public bool HasVideo { get; set; }
        public bool HasSound { get; set; }
    }
}
