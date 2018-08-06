using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Simulation.Game.Serialization
{
    public class SerializationUtils
    {
        private static readonly Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        public static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);

        public static void SerializeType(Type type, ref JObject jObject) => jObject.Add("type", type.FullName);
        public static object GetObjectFromToken(Type type, JToken jToken) => Serializer.Deserialize(new JTokenReader(jToken), type);
        public static JToken GetJTokenFromObject(object obj) => JToken.FromObject(obj, Serializer);

        public static object CreateInstance(string filepath)
        {
            if(filepath.EndsWith(".cs"))
            {
                return GetAssembly(filepath).CreateInstance(Path.GetFileNameWithoutExtension(filepath));
            }
            else
            {
                return Assembly.GetEntryAssembly().CreateInstance(filepath);
            }
        }

        public static Assembly GetAssembly(string filepath)
        {
            if(!loadedAssemblies.ContainsKey(filepath))
            {
                loadedAssemblies[filepath] = ReflectionUtils.LoadAssembly(Path.Combine(Util.Util.GetScriptBasePath(), filepath));
            }

            return loadedAssemblies[filepath];
        }

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

        public static void AddToObject(JObject jObject, string key, object value)
        {
            if (value != null)
            {
                jObject.Add(key, JToken.FromObject(value, Serializer));
            }
            else
            {
                jObject.Add(key, null);
            }
        }

        public static T GetFromObject<T>(JObject jObject, string key, T defaultValue = default(T))
        {
            if (jObject.GetValue(key) == null)
            {
                return defaultValue;
            }

            return (T)Serializer.Deserialize(new JTokenReader(jObject.GetValue(key)), typeof(T));
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
