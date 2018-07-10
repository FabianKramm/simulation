using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class AmbientHitableObjectSerializer: HitableObjectSerializer
    {
        private static readonly Type staticBlockingObjectType = typeof(AmbientHitableObject);
        private static readonly string[] serializeableProperties = new string[] {
            "AmbientHitableObjectType"
        };

        public static AmbientHitableObject Deserialize(JObject jObject)
        {
            AmbientHitableObject staticBlockingObject = ReflectionUtils.CallPrivateConstructor<AmbientHitableObject>();

            Deserialize(ref jObject, staticBlockingObject);

            return staticBlockingObject;
        }

        public static JObject Serialize(AmbientHitableObject staticBlockingObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(staticBlockingObjectType, ref retObject);
            Serialize(staticBlockingObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, AmbientHitableObject staticBlockingObjectleObject)
        {
            HitableObjectSerializer.Deserialize(ref jObject, staticBlockingObjectleObject);

            SerializationUtils.SetFromObject(jObject, staticBlockingObjectleObject, staticBlockingObjectType, serializeableProperties);
        }

        protected static void Serialize(AmbientHitableObject staticBlockingObject, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(staticBlockingObject, ref jObject);

            SerializationUtils.AddToObject(jObject, staticBlockingObject, staticBlockingObjectType, serializeableProperties);
        }
    }
}
