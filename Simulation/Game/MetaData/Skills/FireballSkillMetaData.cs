using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;

namespace Simulation.Game.MetaData.Skills
{
    public class FireballSkillMetaData: SkillMetaData
    {
        public Vector2? RelativeOriginPosition = new Vector2(0, -10);

        public static FireballSkill Create(MovingEntity movingEntity, FireballSkillMetaData fireballSkillMetaData)
        {
            return new FireballSkill(movingEntity, fireballSkillMetaData.RelativeOriginPosition);
        }
    }
}
