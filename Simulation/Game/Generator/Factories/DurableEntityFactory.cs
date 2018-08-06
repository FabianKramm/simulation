using Simulation.Game.MetaData;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;

namespace Simulation.Game.Generator.Factories
{
    public class DurableEntityFactory
    {
        public static DurableEntity CreateGeralt()
        {
            var geralt = (DurableEntity)LivingEntityType.Create(new WorldPosition(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3, Interior.Outside), LivingEntityType.lookup[1]);

            // geralt.BaseAI = new WanderAI(geralt, 10);
            // geralt.BaseAI = new FollowAI(geralt, SimulationGame.Player, WorldGrid.BlockSize.X);

            return geralt;
        }
    }
}
