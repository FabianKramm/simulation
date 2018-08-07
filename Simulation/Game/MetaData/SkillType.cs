using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Scripts.Base;

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
