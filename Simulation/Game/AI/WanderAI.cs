using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using System;

namespace Simulation.Game.AI
{
    public class WanderAI: BaseAI
    {
        private static readonly TimeSpan waitAfterWalking = TimeSpan.FromMilliseconds(1000);
        
        public int BlockRadius;
        public WorldPosition BlockStartPosition;

        public WanderAI(MovingEntity movingEntity, WorldPosition blockStartPosition, int blockRadius): base(movingEntity)
        {
            BlockRadius = blockRadius;
            BlockStartPosition = blockStartPosition;

            behaviorTree = createBehaviorTree();
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    .LongRunningResultCached(() => new WaitTask(Entity, TimeSpan.FromMilliseconds(1000)))
                    .LongRunningResultCached(() => new WanderTask(Entity, BlockStartPosition, BlockRadius))
                .End()
                .Build(),
                Entity
            );
        }
    }
}
