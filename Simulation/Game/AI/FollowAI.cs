using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.AI
{
    public class FollowAI: BaseAI
    {
        private LivingEntity target;
        private float tillDistance;

        public FollowAI(MovingEntity movingEntity, string targetId, float realDistance): base(movingEntity)
        {
            // this.target = target;
            this.tillDistance = realDistance;

            Init();
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    .LongRunningResultCached(() => new WaitTask(Entity, TimeSpan.FromMilliseconds(200)))
                    .LongRunningResultCached(() => new FollowTask(Entity, target, tillDistance))
                .End()
                .Build(),
                Entity
            );
        }
    }
}
