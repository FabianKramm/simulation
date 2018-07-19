using Newtonsoft.Json.Linq;
using Simulation.Game.AI;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization.AI
{
    public class WanderAISerializer: BaseAISerializer
    {
        private static readonly Type type = typeof(WanderAI);
        private static readonly string[] serializeableProperties = new string[] {
            "BlockStartPosition",
            "BlockRadius"
        };

        public static WanderAI Deserialize(JObject jObject, MovingEntity movingEntity)
        {
            WanderAI deserializedObject = ReflectionUtils.CallPrivateConstructor<WanderAI>(new Type[] { typeof(MovingEntity) }, new object[] { movingEntity });

            Deserialize(ref jObject, deserializedObject);

            return deserializedObject;
        }

        public static JObject Serialize(WanderAI serializeObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(serializeObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, WanderAI deserializeObject)
        {
            BaseAISerializer.Deserialize(ref jObject, deserializeObject);

            SerializationUtils.SetFromObject(jObject, deserializeObject, type, serializeableProperties);
        }

        protected static void Serialize(WanderAI serializeObject, ref JObject jObject)
        {
            BaseAISerializer.Serialize(serializeObject, ref jObject);

            SerializationUtils.AddToObject(jObject, serializeObject, type, serializeableProperties);
        }
    }
}
