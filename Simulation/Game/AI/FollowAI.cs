using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.AI
{
    public class FollowAI: BaseAI
    {
        private string targetID;
        private float tillDistance;
        private float teleportDistance;

        public FollowAI(MovingEntity movingEntity, string targetID, float realDistance, float teleportDistance = 0) : base(movingEntity)
        {
            this.targetID = targetID;
            this.tillDistance = realDistance;
            this.teleportDistance = teleportDistance;

            behaviorTree = createBehaviorTree();
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    .LongRunningResultCached(() => new WaitTask(Entity, TimeSpan.FromMilliseconds(200)))
                    .LongRunningResultCached(() => new FollowTask(Entity, targetID, tillDistance, teleportDistance))
                .End()
                .Build(),
                Entity
            );
        }
    }
}
