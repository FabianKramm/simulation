using Simulation.Game.Enums;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;

namespace Simulation.Game.Generator.Factories
{
    public class DurableEntityFactory
    {
        public static DurableEntity CreateGeralt()
        {
            var geralt = new DurableEntity(LivingEntityType.GERALT, new WorldPosition(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3), FractionType.NPC);

            // geralt.BaseAI = new FollowAI(geralt, SimulationGame.Player, 64);

            return geralt;
        }
    }
}
