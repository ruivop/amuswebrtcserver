using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{
    public enum UserChangedStatus
    {
        online,
        offline,
    }

    public class UserStatusChangedMessage
    {
        public UserChangedStatus Status { get; set; }

        public bool HasVideo { get; set; }
        public bool HasSound { get; set; }
    }
}
