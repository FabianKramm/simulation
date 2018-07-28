using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using System;
using System.Collections.Generic;

namespace Simulation.Game.MetaData.Skills
{
    public abstract class SkillMetaData
    {
        private static Dictionary<string, Func<MovingEntity, SkillMetaData, Skill>> skillCreatorLookup = new Dictionary<string, Func<MovingEntity, SkillMetaData, Skill>>()
        {
            {typeof(SlashSkillMetaData).FullName, (MovingEntity movingEntity, SkillMetaData skillMetaData) => SlashSkillMetaData.Create(movingEntity, (SlashSkillMetaData)skillMetaData)},
            {typeof(FireballSkillMetaData).FullName, (MovingEntity movingEntity, SkillMetaData skillMetaData) => FireballSkillMetaData.Create(movingEntity, (FireballSkillMetaData)skillMetaData)}
        };

        public static Skill Create(MovingEntity movingEntity, SkillMetaData skillMetaData) => skillCreatorLookup[skillMetaData.GetType().FullName](movingEntity, skillMetaData);
    }
}
