using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class DurableEntitySerializer: MovingEntitySerializer
    {
        private static readonly Type durableEntityType = typeof(DurableEntity);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(durableEntityType);

        public new static DurableEntity Deserialize(JObject jObject)
        {
            DurableEntity durableEntity = ReflectionUtils.CallPrivateConstructor<DurableEntity>();

            Deserialize(ref jObject, durableEntity);

            return durableEntity;
        }

        public static JObject Serialize(DurableEntity durableEntity)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(durableEntityType, ref retObject);
            Serialize(durableEntity, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, DurableEntity durableEntity)
        {
            MovingEntitySerializer.Deserialize(ref jObject, durableEntity, true);

            SerializationUtils.SetFromObject(jObject, durableEntity, durableEntityType, serializeableProperties);

            durableEntity.Init();
        }

        protected static void Serialize(DurableEntity durableEntity, ref JObject jObject)
        {
            MovingEntitySerializer.Serialize(durableEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, durableEntity, durableEntityType, serializeableProperties);
        }
    }
}
