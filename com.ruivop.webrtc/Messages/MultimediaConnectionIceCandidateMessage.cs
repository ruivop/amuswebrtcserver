using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{
    public class MultimediaConnectionIceCandidateMessage
    {
        public string UsernameTo { get; set; }
        public string Candidate { get; set; }
    }
}
