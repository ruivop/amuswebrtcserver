using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Helpers
{
    public static class Globals
    {
        public static readonly int MAX_USER_MESSAGE_LENGTH = 1024 * 1024;

        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }
}
