using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventSourcingCQRS
{
    public class LogParser
    {
        public static string ConvertToHistory(object obj) => JsonConvert.SerializeObject(obj);

        public static T ConvertStringToObject<T>(string history) => JsonConvert.DeserializeObject<T>(history);
    }
}
