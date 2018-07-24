using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using System;

namespace Simulation.Game.AI
{
    public class WanderAI: BaseAI
    {
        private static readonly TimeSpan waitAfterWalking = TimeSpan.FromMilliseconds(1000);

        public int BlockRadius
        {
            get; private set;
        }

        public WorldPosition BlockStartPosition;

        // From JSON
        private WanderAI(MovingEntity movingEntity): base(movingEntity) { }

        public WanderAI(MovingEntity movingEntity, int blockRadius): base(movingEntity)
        {
            BlockRadius = blockRadius;
            BlockStartPosition = new WorldPosition(movingEntity.BlockPosition.X, movingEntity.BlockPosition.Y, movingEntity.Position.InteriorID);

            Init();
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
