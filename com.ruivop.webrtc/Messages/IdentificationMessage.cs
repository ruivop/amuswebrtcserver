using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{
    public class IdentificationMessage
    {
        public string Username { get; set; }
        public string Color { get; set; }
        public bool HasSound { get; set; }
        public bool HasVideo { get; set; }
    }
}
