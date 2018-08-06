using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class LivingEntitySerializer: HitableObjectSerializer
    {
        private static readonly Type type = typeof(LivingEntity);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(type);

        protected static void Deserialize(ref JObject jObject, LivingEntity livingEntity)
        {
            HitableObjectSerializer.Deserialize(ref jObject, livingEntity);

            SerializationUtils.SetFromObject(jObject, livingEntity, type, serializeableProperties);
        }

        protected static void Serialize(LivingEntity livingEntity, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(livingEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, livingEntity, type, serializeableProperties);
        }
    }
}
