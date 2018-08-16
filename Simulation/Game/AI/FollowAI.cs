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

        public FollowAI(MovingEntity movingEntity, string targetID, float realDistance): base(movingEntity)
        {
            this.targetID = targetID;
            this.tillDistance = realDistance;

            behaviorTree = createBehaviorTree();
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    .LongRunningResultCached(() => new WaitTask(Entity, TimeSpan.FromMilliseconds(200)))
                    .LongRunningResultCached(() => new FollowTask(Entity, targetID, tillDistance))
                .End()
                .Build(),
                Entity
            );
        }
    }
}
