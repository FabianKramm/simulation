using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.AITasks
{
    public class WaitTask: AITask
    {
        private TimeSpan waited;

        public WaitTask(LivingEntity subject, TimeSpan waitTime): base(subject)
        {
            waited = waitTime;
        }

        public override void Update(GameTime gameTime)
        {
            if(IsStarted)
            {
                waited -= gameTime.ElapsedGameTime;

                if (waited.TotalMilliseconds <= 0)
                {
                    IsFinished = true;
                }
            }
        }
    }
}
