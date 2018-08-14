using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class AmbientHitableObjectSerializer: HitableObjectSerializer
    {
        private static readonly Type type = typeof(AmbientHitableObject);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(type);

        public static AmbientHitableObject Deserialize(JObject jObject)
        {
            AmbientHitableObject ambientHitableObject = ReflectionUtils.CallPrivateConstructor<AmbientHitableObject>();

            Deserialize(ref jObject, ambientHitableObject);

            ambientHitableObject.Init();

            return ambientHitableObject;
        }

        public static JObject Serialize(AmbientHitableObject ambientHitableObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(ambientHitableObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, AmbientHitableObject ambientHitableObject)
        {
            HitableObjectSerializer.Deserialize(ref jObject, ambientHitableObject);

            SerializationUtils.SetFromObject(jObject, ambientHitableObject, type, serializeableProperties);
        }

        protected static void Serialize(AmbientHitableObject staticBlockingObject, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(staticBlockingObject, ref jObject);

            SerializationUtils.AddToObject(jObject, staticBlockingObject, type, serializeableProperties);
        }
    }
}
