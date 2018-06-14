using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Spritesheet;
using System;

namespace Simulation.Game
{
    class Player
    {
        Simulation.Spritesheet.Spritesheet sheet;
        WalkingDirection curDirection = WalkingDirection.Idle;
        Animation curAnimation = null;

        public void LoadContent(ContentManager content)
        {
            Texture2D texture = content.Load<Texture2D>("player");
            sheet = new Simulation.Spritesheet.Spritesheet(texture).WithGrid((64, 64)).WithFrameDuration(120);

            curAnimation = getAnimation(curDirection);
        }

        private int getAnimationOffset(WalkingDirection direction)
        {
            int offset = 10; /* 8 top 9 left 10 down 11 right */

            if ((direction & WalkingDirection.Left) == WalkingDirection.Left)
            {
                offset = 9;
            }
            else if ((direction & WalkingDirection.Right) == WalkingDirection.Right)
            {
                offset = 11;
            }
            else if ((direction & WalkingDirection.Up) == WalkingDirection.Up)
            {
                offset = 8;
            }

            return offset;
        }

        private Animation getAnimation(WalkingDirection direction)
        {
            int offset = getAnimationOffset(direction);

            // anim.Start(Repeat.Mode.Loop);
            return sheet.CreateAnimation((0, offset), (1, offset), (2, offset), (3, offset), (4, offset), (5, offset), (6, offset), (7, offset), (8, offset)); 
        }


        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            WalkingDirection oldDirection = curDirection;
            int oldAnimationOffset = getAnimationOffset(curDirection);
            int newAnimationOffset = 0;

            if (state.IsKeyDown(Keys.Right))
            {
                SimulationGame.camera.Position += new Vector2(5, 0);
                curDirection = curDirection | WalkingDirection.Right;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Right);
            }

            if (state.IsKeyDown(Keys.Left))
            {
                SimulationGame.camera.Position += new Vector2(-5, 0);
                curDirection = curDirection | WalkingDirection.Left;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Left);
            }

            if (state.IsKeyDown(Keys.Up))
            {
                SimulationGame.camera.Position += new Vector2(0, -5);
                curDirection = curDirection | WalkingDirection.Up;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Up);
            }

            if (state.IsKeyDown(Keys.Down))
            {
                SimulationGame.camera.Position += new Vector2(0, 5);
                curDirection = curDirection | WalkingDirection.Down;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Down);
            }

            newAnimationOffset = getAnimationOffset(curDirection);

            if (curDirection == WalkingDirection.Idle)
            {
                curAnimation.Stop();
            } 
            else if(newAnimationOffset != oldAnimationOffset)
            {
                curAnimation = getAnimation(curDirection);
                curAnimation.Start(Repeat.Mode.Loop);
            }
            else if(oldDirection == WalkingDirection.Idle && curDirection != WalkingDirection.Idle)
            {
                if(curDirection == WalkingDirection.Down)
                {
                    curAnimation = getAnimation(curDirection);
                }

                curAnimation.Start(Repeat.Mode.Loop);
            }

            curAnimation.Update(gameTime);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation, SimulationGame.camera.Position);
        }
    }
}
