using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartClass.Common
{
    public static partial class MyExtensions
    {
        class IPAddressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPAddress));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                IPAddress ip = (IPAddress)value;
                writer.WriteValue(ip.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken token = JToken.Load(reader);
                return IPAddress.Parse(token.Value<string>());
            }
        }
        class IPEndPointConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPEndPoint));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                IPEndPoint ep = (IPEndPoint)value;
                writer.WriteStartObject();
                writer.WritePropertyName("Address");
                serializer.Serialize(writer, ep.Address);
                writer.WritePropertyName("Port");
                writer.WriteValue(ep.Port);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
                int port = jo["Port"].Value<int>();
                return new IPEndPoint(address, port);
            }
        }
        /// <summary>
        /// object as json
        /// </summary>
        /// <param name="model"></param>
        /// <param name="indented"></param>
        /// <returns></returns>
        public static string ToJson(this object model, bool indented = false)
        {
            if (model is string)
            {
                return model.ToString();
            }
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new IPEndPointConverter());
            settings.Formatting = indented ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(model, settings);
        }

        /// <summary>
        /// json as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this string json, bool ThrowException = false)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                if (ThrowException)
                {
                    throw;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// json as object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToEntity(this string json, object defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            try
            {
                return JsonConvert.DeserializeObject(json);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static IDictionary<string, object> ToDictionary(this object theObject, bool alwaysCreateDefaultDictionary = true)
        {
            if (theObject == null)
            {
                return alwaysCreateDefaultDictionary ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) : null;
            }

            if (theObject is JObject jObject)
            {
                return jObject.ToDictionary();
            }

            var json = JsonConvert.SerializeObject(theObject);
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            return dictionary;
        }

        public static TValue TryGetValueAs<TValue>(this IDictionary<string, object> dictionary, string key, TValue defaultValue)
        {
            if (dictionary == null)
            {
                return defaultValue;
            }

            var theKey = dictionary.Keys.SingleOrDefault(x => x.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (theKey == null)
            {
                return defaultValue;
            }

            var convertTo = SimpleConvert.SafeConvertTo<TValue>(dictionary[theKey]);
            return convertTo;
        }

        private static IDictionary<string, object> ToDictionary(this JObject @object)
        {
            var result = @object.ToObject<Dictionary<string, object>>();

            var JObjectKeys = (from r in result
                               let key = r.Key
                               let value = r.Value
                               where value.GetType() == typeof(JObject)
                               select key).ToList();

            var JArrayKeys = (from r in result
                              let key = r.Key
                              let value = r.Value
                              where value.GetType() == typeof(JArray)
                              select key).ToList();

            JArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
            JObjectKeys.ForEach(key => result[key] = ToDictionary(result[key] as JObject));

            return result;
        }
    }
}
