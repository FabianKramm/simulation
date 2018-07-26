using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Enums;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using Simulation.Game.World;

namespace Simulation.Game.Generator.Factories
{
    public class DurableEntityFactory
    {
        public static DurableEntity CreateGeralt()
        {
            var geralt = new DurableEntity(LivingEntityType.GERALT, new WorldPosition(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3, Interior.Outside), FractionType.NPC);

            geralt.Skills = new Skill[2]
            {
                new FireballSkill(geralt, new Vector2(0, -10)),
                new SlashSkill(geralt, new Vector2(0, -14))
            };

            // geralt.BaseAI = new WanderAI(geralt, 10);
            geralt.BaseAI = new FollowAI(geralt, SimulationGame.Player, WorldGrid.BlockSize.X);

            return geralt;
        }
    }
}
