using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization
{
    public class SerializationUtils
    {
        public static readonly JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            });

        public static void SerializeType(Type type, ref JObject jObject) => jObject.Add("type", type.FullName);
        public static object GetObjectFromToken(Type type, JToken jToken) => serializer.Deserialize(new JTokenReader(jToken), type);
        public static JToken GetJTokenFromObject(object obj) => JToken.FromObject(obj, serializer);

        public static void AddToObject(JObject jObject, object obj, Type type, string[] names)
        {
            foreach(var propertyName in names) 
                jObject.Add(propertyName, JToken.FromObject(ReflectionUtils.GetMemberValue(type, obj, propertyName), serializer));
        }

        public static void AddToObject(JObject jObject, object obj, Type type, string propertyName)
        {
            jObject.Add(propertyName, JToken.FromObject(ReflectionUtils.GetMemberValue(type, obj, propertyName), serializer));
        }

        public static void SetFromObject(JObject jObject, object obj, Type type, string[] names)
        {
            foreach (var propertyName in names)
                ReflectionUtils.SetMemberValue(type, obj, propertyName, serializer.Deserialize(new JTokenReader(jObject.GetValue(propertyName)), ReflectionUtils.GetMemberType(type, propertyName)));
        }

        public static void SetFromObject(JObject jObject, object obj, Type type, string propertyName)
        {
            ReflectionUtils.SetMemberValue(type, obj, propertyName, serializer.Deserialize(new JTokenReader(jObject.GetValue(propertyName)), ReflectionUtils.GetMemberType(type, propertyName)));
        }
    }
}
