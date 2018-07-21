using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization.Skills
{
    public class FireballSkillSerializer: SkillSerializer
    {
        private static readonly Type type = typeof(FireballSkill);
        private static readonly string[] serializeableProperties = new string[] {
            "relativeOriginPosition"
        };

        public static SlashSkill Deserialize(JObject jObject, LivingEntity deserialize)
        {
            SlashSkill deserializedObject = ReflectionUtils.CallPrivateConstructor<SlashSkill>(new Type[] { typeof(LivingEntity) }, new object[] { deserialize });

            Deserialize(ref jObject, deserializedObject);

            return deserializedObject;
        }

        public static JObject Serialize(FireballSkill serializeObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(serializeObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, FireballSkill deserializeObject)
        {
            SkillSerializer.Deserialize(ref jObject, deserializeObject);

            SerializationUtils.SetFromObject(jObject, deserializeObject, type, serializeableProperties);
        }

        protected static void Serialize(FireballSkill serializeObject, ref JObject jObject)
        {
            SkillSerializer.Serialize(serializeObject, ref jObject);

            SerializationUtils.AddToObject(jObject, serializeObject, type, serializeableProperties);
        }
    }
}
