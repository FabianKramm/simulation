using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.Basics;
using Simulation.Game.Hud;
using Simulation.Game.Skills;
using Simulation.Spritesheet;
using Simulation.Util;
using System;

namespace Simulation.Game
{
    public class Player: LivingEntity
    {
        Simulation.Spritesheet.Spritesheet sheet;
        WalkingDirection curDirection = WalkingDirection.Idle;
        Animation curAnimation = null;

        private float velocity = 0.3f;
        private FireballSkill fireballSkill;

        public Player(): base(new Vector2(0, 0), new Point(-10, -30), new Point(20, 30))
        {
            fireballSkill = new FireballSkill(this, new Vector2(0, -30));
        }

        public void LoadContent()
        {
            Texture2D texture = SimulationGame.contentManager.Load<Texture2D>("player");
            sheet = new Simulation.Spritesheet.Spritesheet(texture).WithGrid((64, 64)).WithCellOrigin(new Point(32, 64)).WithFrameDuration(120);

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

            Vector2 newPosition = position;

            if (state.IsKeyDown(Keys.D))
            {
                newPosition.X += (int)(velocity * gameTime.ElapsedGameTime.Milliseconds);
                curDirection = curDirection | WalkingDirection.Right;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Right);
            }

            if (state.IsKeyDown(Keys.A))
            {
                newPosition.X -= (int)(velocity * gameTime.ElapsedGameTime.Milliseconds);
                curDirection = curDirection | WalkingDirection.Left;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Left);
            }

            if (state.IsKeyDown(Keys.W))
            {
                newPosition.Y -= (int)(velocity * gameTime.ElapsedGameTime.Milliseconds);
                curDirection = curDirection | WalkingDirection.Up;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Up);
            }

            if (state.IsKeyDown(Keys.S))
            {
                newPosition.Y += (int)(velocity * gameTime.ElapsedGameTime.Milliseconds);
                curDirection = curDirection | WalkingDirection.Down;
            }
            else
            {
                curDirection = curDirection & (WalkingDirection.Mask ^ WalkingDirection.Down);
            }

            if (state.IsKeyDown(Keys.D1))
            {
                fireballSkill.use(SimulationGame.mousePosition);
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

            if((position.X != newPosition.X || position.Y != newPosition.Y) && canMove(newPosition))
            {
                SimulationGame.world.removeCollidableObject(this);

                position = newPosition;
                SimulationGame.camera.Position = new Vector2(position.X, position.Y);

                SimulationGame.world.addCollidableObject(this);
            }

            fireballSkill.Update(gameTime);
            curAnimation.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation, position, layerDepth: GeometryUtils.getLayerDepthFromYPosition(position.Y));

            base.Draw(spriteBatch);
        }
    }
}
