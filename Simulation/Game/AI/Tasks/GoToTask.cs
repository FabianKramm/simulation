using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using System.Linq;

namespace Simulation.Game.AI.Tasks
{
    public class GoToTask: BehaviorTask
    {
        private WorldPosition goToBlockPosition;

        public GoToTask(MovingEntity subject, WorldPosition goToBlockPosition) : base(subject)
        {
            this.goToBlockPosition = goToBlockPosition;
        }
        
        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingSubject = (MovingEntity)subject;

            if (!movingSubject.IsWalking)
            {
                if (movingSubject.BlockPosition.X != goToBlockPosition.X || movingSubject.BlockPosition.Y != goToBlockPosition.Y || movingSubject.InteriorID != goToBlockPosition.InteriorID)
                {
                    movingSubject.WalkToBlock(goToBlockPosition);
                }
                else
                {
                    return BehaviourTreeStatus.Success;
                }
            }

            return BehaviourTreeStatus.Running;
        }
    }
}
