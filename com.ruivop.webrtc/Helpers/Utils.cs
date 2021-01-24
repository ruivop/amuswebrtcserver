using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.ruivop.webrtc.Helpers
{
    public static class Utils
    {
        public static T JsonElementToObject<T>(object element)
        {
            var json = ((JsonElement)element).GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
