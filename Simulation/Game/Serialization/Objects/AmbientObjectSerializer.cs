using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class AmbientObjectSerializer: GameObjectSerializer
    {
        private static readonly Type ambientObjectType = typeof(AmbientObject);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(ambientObjectType);

        public static AmbientObject Deserialize(JObject jObject)
        {
            AmbientObject ambientObject = ReflectionUtils.CallPrivateConstructor<AmbientObject>();

            Deserialize(ref jObject, ambientObject);

            return ambientObject;
        }

        public static JObject Serialize(AmbientObject obj)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(ambientObjectType, ref retObject);
            Serialize(obj, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, AmbientObject ambientObject)
        {
            GameObjectSerializer.Deserialize(ref jObject, ambientObject);

            SerializationUtils.SetFromObject(jObject, ambientObject, ambientObjectType, serializeableProperties);
        }

        protected static void Serialize(AmbientObject ambientObject, ref JObject jObject)
        {
            GameObjectSerializer.Serialize(ambientObject, ref jObject);

            SerializationUtils.AddToObject(jObject, ambientObject, ambientObjectType, serializeableProperties);
        }
    }
}
