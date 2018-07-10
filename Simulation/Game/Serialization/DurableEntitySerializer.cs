using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class DurableEntitySerializer: MovingEntitySerializer
    {
        private static readonly Type durableEntityType = typeof(DurableEntity);
        private static readonly string[] serializeableProperties = new string[] {
            "preloadedSurroundingWorldGridChunkRadius",
            "PreloadedWorldGridChunkBounds",
            "PreloadedWorldGridChunkPixelBounds"
        };

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
            LivingEntitySerializer.Deserialize(ref jObject, durableEntity);

            SerializationUtils.SetFromObject(jObject, durableEntity, durableEntityType, serializeableProperties);
        }

        protected static void Serialize(DurableEntity durableEntity, ref JObject jObject)
        {
            LivingEntitySerializer.Serialize(durableEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, durableEntity, durableEntityType, serializeableProperties);
        }
    }
}
