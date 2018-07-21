using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization.AI;
using Simulation.Game.Skills;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization.Skills
{
    public class SlashSkillSerializer: SkillSerializer
    {
        private static readonly Type type = typeof(SlashSkill);
        private static readonly string[] serializeableProperties = new string[] {
            "relativeOriginPosition"
        };

        public static SlashSkill Deserialize(JObject jObject, LivingEntity deserialize)
        {
            SlashSkill deserializedObject = ReflectionUtils.CallPrivateConstructor<SlashSkill>(new Type[] { typeof(MovingEntity) }, new object[] { deserialize });

            Deserialize(ref jObject, deserializedObject);

            return deserializedObject;
        }

        public static JObject Serialize(SlashSkill serializeObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(serializeObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, SlashSkill deserializeObject)
        {
            SkillSerializer.Deserialize(ref jObject, deserializeObject);

            SerializationUtils.SetFromObject(jObject, deserializeObject, type, serializeableProperties);
        }

        protected static void Serialize(SlashSkill serializeObject, ref JObject jObject)
        {
            SkillSerializer.Serialize(serializeObject, ref jObject);

            SerializationUtils.AddToObject(jObject, serializeObject, type, serializeableProperties);
        }
    }
}
