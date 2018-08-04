using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Interfaces;
using Simulation.Util;
using System;
using System.IO;

namespace Simulation.Game.Serialization.Objects
{
    public class AmbientObjectSerializer: GameObjectSerializer
    {
        private static readonly Type type = typeof(AmbientObject);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(type);

        public static AmbientObject Deserialize(JObject jObject)
        {
            AmbientObject ambientObject = ReflectionUtils.CallPrivateConstructor<AmbientObject>();

            Deserialize(ref jObject, ambientObject);

            return ambientObject;
        }

        public static JObject Serialize(AmbientObject obj)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(obj, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, AmbientObject ambientObject)
        {
            GameObjectSerializer.Deserialize(ref jObject, ambientObject);

            SerializationUtils.SetFromObject(jObject, ambientObject, type, serializeableProperties);

            var ambientObjectType = AmbientObjectType.lookup[ambientObject.AmbientObjectType];

            if (ambientObjectType.CustomControllerScript != null)
            {
                ambientObject.CustomController = (GameObjectController)SerializationUtils
                    .GetAssembly(ambientObjectType.CustomControllerScript)
                    .CreateInstance(Path.GetFileNameWithoutExtension(ambientObjectType.CustomControllerScript));
            }

            if (ambientObjectType.CustomRendererScript != null)
            {
                ambientObject.CustomRenderer = (GameObjectRenderer)SerializationUtils
                    .GetAssembly(ambientObjectType.CustomRendererScript)
                    .CreateInstance(Path.GetFileNameWithoutExtension(ambientObjectType.CustomRendererScript));
            }

            ambientObject.Init();
        }

        protected static void Serialize(AmbientObject ambientObject, ref JObject jObject)
        {
            GameObjectSerializer.Serialize(ambientObject, ref jObject);

            SerializationUtils.AddToObject(jObject, ambientObject, type, serializeableProperties);
        }
    }
}
