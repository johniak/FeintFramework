

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

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

        public static Type getTypeOfFieldOrProperty(Type type, string name)
        {
            var member = type.GetMember(name)[0];
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            else
            {
                return ((PropertyInfo)member).PropertyType;
            }
        }
    }
}