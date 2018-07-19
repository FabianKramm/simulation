using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization.AI;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class LivingEntitySerializer: HitableObjectSerializer
    {
        private static readonly Type livingEntityType = typeof(LivingEntity);
        private static readonly string[] serializeableProperties = new string[] {
            "LivingEntityType",
            "Fraction"
        };

        protected static void Deserialize(ref JObject jObject, LivingEntity livingEntity)
        {
            HitableObjectSerializer.Deserialize(ref jObject, livingEntity);

            SerializationUtils.SetFromObject(jObject, livingEntity, livingEntityType, serializeableProperties);

            if(jObject.GetValue("BaseAI") != null)
            {
                livingEntity.BaseAI = AISerializer.Deserialize((JObject)jObject.GetValue("BaseAI"), livingEntity);
            }
        }

        protected static void Serialize(LivingEntity livingEntity, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(livingEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, livingEntity, livingEntityType, serializeableProperties);

            jObject.Add("BaseAI", livingEntity.BaseAI != null ? AISerializer.Serialize(livingEntity.BaseAI) : null);
        }
    }
}
