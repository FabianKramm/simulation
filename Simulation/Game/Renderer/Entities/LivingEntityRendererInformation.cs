using Microsoft.Xna.Framework;
using Simulation.Spritesheet;
using Simulation.Util;

namespace Simulation.Game.Renderer.Entities
{
    public class LivingEntityRendererInformation
    {
        public WalkingDirection walkingDirection
        {
            get; private set;
        }

        public Animation currentAnimation;

        private Animation Down;
        private Animation Up;
        private Animation Left;
        private Animation Right;

        public LivingEntityRendererInformation(Animation Down, Animation Up, Animation Left, Animation Right)
        {
            this.Down = Down;
            this.Up = Up;
            this.Left = Left;
            this.Right = Right;

            currentAnimation = Down;
        }

        public void Update(GameTime gameTime, WalkingDirection newWalkingDirection)
        { 
            if(newWalkingDirection == WalkingDirection.Idle)
            {
                walkingDirection = WalkingDirection.Idle;
                currentAnimation.Stop();
            }
            else if (newWalkingDirection != walkingDirection)
            {
                walkingDirection = newWalkingDirection;
                currentAnimation.Stop();

                if (newWalkingDirection == WalkingDirection.Down)
                    currentAnimation = Down;
                if (newWalkingDirection == WalkingDirection.Up)
                    currentAnimation = Up;
                if (newWalkingDirection == WalkingDirection.Left)
                    currentAnimation = Left;
                if (newWalkingDirection == WalkingDirection.Right)
                    currentAnimation = Right;
                
                currentAnimation.Start(Repeat.Mode.Loop);
            }

            currentAnimation.Update(gameTime);
        }
    }
}
