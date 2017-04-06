

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FeintApi
{
    public static class Helpers
    {
        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string objectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }
    }
}