using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using FeintSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using Newtonsoft.Json.Converters;
namespace FeintSDK.Server
{
    class ComplexContentConverter : CustomCreationConverter<object>
    {
        public override object Create(Type objectType)
        {
            if (objectType == typeof(List<object>))
            {
                return new List<object>();
            }
            return new Dictionary<string, object>();
        }

        public override bool CanConvert(Type objectType)
        {
            // in addition to handling IDictionary<string, object>
            // we want to handle the deserialization of dict value
            // which is of type object
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject
                || reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);
            if (reader.TokenType == JsonToken.StartArray)
            {
                return base.ReadJson(reader, typeof(List<object>), existingValue, serializer);
            }
            // if the next token is not an object
            // then fall back on standard deserializer (strings, numbers etc.)
            return serializer.Deserialize(reader);
        }
    }
}