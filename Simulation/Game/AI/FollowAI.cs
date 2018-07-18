using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.AI
{
    public class FollowAI: BaseAI
    {
        public FollowAI(MovingEntity movingEntity): base(movingEntity) { }

        protected override void addAITasks()
        {
            throw new NotImplementedException();
        }
    }
}
