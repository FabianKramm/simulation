using Simulation.Game.AI.AITasks;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

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

            return builder
                .Sequence()
                    .LongRunning(() => new WaitTask(Entity, TimeSpan.FromMilliseconds(1000)))
                    .LongRunning(() => new WanderTask(Entity, BlockStartPosition, BlockRadius))
                .End()
                .Build();
        }
    }
}
