using Newtonsoft.Json.Linq;
using Simulation.Game.World;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class InteriorSerializer: WorldPartSerialization
    {
        private static readonly Type interiorType = typeof(Interior);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(interiorType);

        public static Interior Deserialize(JObject jObject)
        {
            Interior interior = ReflectionUtils.CallPrivateConstructor<Interior>();

            Deserialize(ref jObject, interior);

            return interior;
        }

        public static JObject Serialize(Interior interior)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(interiorType, ref retObject);
            Serialize(interior, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, Interior interior)
        {
            WorldPartSerialization.Deserialize(ref jObject, interior);

            SerializationUtils.SetFromObject(jObject, interior, interiorType, serializeableProperties);
        }

        protected static void Serialize(Interior interior, ref JObject jObject)
        {
            WorldPartSerialization.Serialize(interior, ref jObject);

            SerializationUtils.AddToObject(jObject, interior, interiorType, serializeableProperties);
        }
    }
}
