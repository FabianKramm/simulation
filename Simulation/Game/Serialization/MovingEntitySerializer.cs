using Newtonsoft.Json.Linq;
using Simulation.Game.Base.Entity;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class MovingEntitySerializer: LivingEntitySerializer
    {
        private static readonly Type movingEntityType = typeof(MovingEntity);
        private static readonly string[] serializeableProperties = new string[] {
            "velocity",
            "direction"
        };

        public static MovingEntity Deserialize(JObject jObject)
        {
            MovingEntity movingEntity = ReflectionUtils.CallPrivateConstructor<MovingEntity>();

            Deserialize(ref jObject, movingEntity);

            return movingEntity;
        }

        public static JObject Serialize(MovingEntity movingEntity)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(movingEntityType, ref retObject);
            Serialize(movingEntity, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, MovingEntity movingEntity)
        {
            LivingEntitySerializer.Deserialize(ref jObject, movingEntity);

            SerializationUtils.SetFromObject(jObject, movingEntity, movingEntityType, serializeableProperties);
        }

        protected static void Serialize(MovingEntity movingEntity, ref JObject jObject)
        {
            LivingEntitySerializer.Serialize(movingEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, movingEntity, movingEntityType, serializeableProperties);
        }
    }
}
