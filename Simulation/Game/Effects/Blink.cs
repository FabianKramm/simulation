using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Spritesheet;

namespace Simulation.Game.Effects
{
    public class Blink: Effect
    {
        public Animation BlinkAnimation;
        private WorldPosition targetPosition;

        public Blink(LivingEntity origin, WorldPosition targetPosition) : base(origin.Position, origin)
        {
            ((MovingEntity)origin).CanWalk = false;

            this.targetPosition = targetPosition;
        }

        private void moveOrigin()
        {
            var movingEntity = ((MovingEntity)Origin);

            if (movingEntity.CanMove(targetPosition))
                movingEntity.UpdatePosition(targetPosition);

            IsFinished = true;
            movingEntity.CanWalk = true;
        }

        public override void Update(GameTime gameTime)
        {
            if(Origin.InteriorID == SimulationGame.Player.InteriorID && SimulationGame.VisibleArea.Contains(Origin.Position))
            {
                if(BlinkAnimation != null)
                {
                    BlinkAnimation.Update(gameTime);

                    if(BlinkAnimation.IsStarted == false)
                        moveOrigin();
                }
                else 
                {
                    moveOrigin();
                }
            }
            else
            {
                moveOrigin();
            }
        }
    }
}
