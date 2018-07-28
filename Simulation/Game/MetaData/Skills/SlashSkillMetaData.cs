using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;

namespace Simulation.Game.MetaData.Skills
{
    public class SlashSkillMetaData: SkillMetaData
    {
        public float DamagePerHit = 10;
        public Vector2? RelativeOriginPosition = new Vector2(0, -14);

        public static SlashSkill Create(MovingEntity movingEntity, SlashSkillMetaData slashSkillMetaData)
        {
            return new SlashSkill(movingEntity, slashSkillMetaData.DamagePerHit, slashSkillMetaData.RelativeOriginPosition);
        }
    }
}
