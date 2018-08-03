using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Interfaces;
using Simulation.Util;
using System;

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

            if (ambientObjectType.CustomControllerAssembly != null)
            {
                ambientObject.CustomController = (GameObjectController)SerializationUtils.GetAssembly(ambientObjectType.CustomControllerAssembly).GetType("CustomController").GetMethod("Create").Invoke(null, new object[] { ambientObject });
            }

            if (ambientObjectType.CustomRendererAssembly != null)
            {
                ambientObject.CustomRenderer = (GameObjectRenderer)SerializationUtils.GetAssembly(ambientObjectType.CustomRendererAssembly).GetType("CustomRenderer").GetMethod("Create").Invoke(null, new object[] { ambientObject });
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
