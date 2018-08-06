using Newtonsoft.Json.Linq;
using Scripts.Base;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;

namespace Simulation.Game.MetaData
{
    public class SkillType
    {
        public string SkillClass = null;
        public JObject SkillParameter = null;

        public static Skill Create(LivingEntity livingEntity, SkillType skillType)
        {
            var skill = (Skill)SerializationUtils.CreateInstance(skillType.SkillClass);

            skill.Init(livingEntity, skillType.SkillParameter == null ? new JObject() : skillType.SkillParameter);

            return skill;
        }
    }
}
