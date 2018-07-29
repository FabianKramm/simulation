using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Simulation.Game.Serialization
{
    public class SerializationUtils
    {
        public static readonly JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.All
        });

        public static void SerializeType(Type type, ref JObject jObject) => jObject.Add("type", type.FullName);
        public static object GetObjectFromToken(Type type, JToken jToken) => Serializer.Deserialize(new JTokenReader(jToken), type);
        public static JToken GetJTokenFromObject(object obj) => JToken.FromObject(obj, Serializer);

        public static string[] GetSerializeables(Type type)
        {
            List<string> retFields = new List<string>();

            var properties = type.GetProperties(ReflectionUtils.Flags | BindingFlags.DeclaredOnly);

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(SerializeAttribute)))
                    retFields.Add(property.Name);
            }

            var fields = type.GetFields(ReflectionUtils.Flags | BindingFlags.DeclaredOnly);

            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, typeof(SerializeAttribute)))
                    retFields.Add(field.Name);
            }

            return retFields.ToArray();
        }

        public static void AddToObject(JObject jObject, object obj, Type type, string[] names)
        {
            foreach(var propertyName in names)
            {
                object value = ReflectionUtils.GetMemberValue(type, obj, propertyName);

                if(value != null)
                {
                    jObject.Add(propertyName, JToken.FromObject(value, Serializer));
                }
                else
                {
                    jObject.Add(propertyName, null);
                }
            }
        }

        public static void SetFromObject(JObject jObject, object obj, Type type, string[] names)
        {
            foreach (var propertyName in names)
                ReflectionUtils.SetMemberValue(type, obj, propertyName, Serializer.Deserialize(new JTokenReader(jObject.GetValue(propertyName)), ReflectionUtils.GetMemberType(type, propertyName)));
        }
    }
}
