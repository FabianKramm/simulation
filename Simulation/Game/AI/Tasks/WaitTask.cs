using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.AI.Tasks
{
    public class WaitTask: BehaviorTask
    {
        private TimeSpan waited;

        public WaitTask(LivingEntity subject, TimeSpan waitTime): base(subject)
        {
            waited = waitTime;
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            waited -= gameTime.ElapsedGameTime;

            if (waited.TotalMilliseconds <= 0)
            {
                return BehaviourTreeStatus.Success;
            }

            return BehaviourTreeStatus.Running;
        }
    }
}
