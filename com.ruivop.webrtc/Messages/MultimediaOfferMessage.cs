using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Messages
{
    public class MultimediaOfferMessage
    {
        public string UsernameTo { get; set; }
        public bool IsClient { get; set; }
        public string Offer { get; set; }
    }
}
