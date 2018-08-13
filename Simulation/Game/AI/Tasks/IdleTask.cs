using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;

namespace Simulation.Game.AI.Tasks
{
    public class IdleTask : BehaviorTask
    {
        private Vector2? lookDirection;

        public IdleTask(MovingEntity subject, Vector2? lookDirection = null) : base(subject)
        {
            this.lookDirection = lookDirection;
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingSubject = (MovingEntity)subject;

            if(lookDirection != null)
            {
                movingSubject.Direction = lookDirection ?? Vector2.Zero;
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
