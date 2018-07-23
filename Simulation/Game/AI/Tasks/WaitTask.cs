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

        public override void Update(GameTime gameTime)
        {
            waited -= gameTime.ElapsedGameTime;

            if (waited.TotalMilliseconds <= 0)
            {
                Status = BehaviourTreeStatus.Success;
            }
        }
    }
}
