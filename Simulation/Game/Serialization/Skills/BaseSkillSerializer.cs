using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Serialization.Skills
{
    public class BaseSkillSerializer
    {
        private static Dictionary<string, Func<JObject, LivingEntity, Skill>> skillDeserializationLookup = new Dictionary<string, Func<JObject, LivingEntity, Skill>>()
        {
            {typeof(SlashSkill).FullName, (JObject jObject, LivingEntity livingEntity) => SlashSkillSerializer.Deserialize(jObject, (LivingEntity)livingEntity)},
            {typeof(FireballSkill).FullName, (JObject jObject, LivingEntity livingEntity) => FireballSkillSerializer.Deserialize(jObject, (LivingEntity)livingEntity)}
        };

        private static Dictionary<string, Func<Skill, JObject>> skillSerializationLookup = new Dictionary<string, Func<Skill, JObject>>()
        {
            {typeof(SlashSkill).FullName, (Skill slashSkill) => SlashSkillSerializer.Serialize((SlashSkill)slashSkill)},
            {typeof(FireballSkill).FullName, (Skill slashSkill) => FireballSkillSerializer.Serialize((FireballSkill)slashSkill)}
        };

        public static JObject Serialize(Skill skill) => skillSerializationLookup[skill.GetType().FullName](skill);
        public static Skill Deserialize(JObject jObject, LivingEntity livingEntity) => skillDeserializationLookup[jObject.GetValue("type").ToString()](jObject, livingEntity);
    }
}
