using Simulation.Game.AI;
using Simulation.Game.Objects.Entities;

namespace Simulation.Game.MetaData.AI
{
    public class WanderAIMetaData: AIMetaData
    {
        public int BlockRadius;

        public static WanderAI Create(MovingEntity movingEntity, WanderAIMetaData wanderAIMetaData)
        {
            return new WanderAI(movingEntity, wanderAIMetaData.BlockRadius);
        }
    }
}
