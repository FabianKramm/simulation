using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.Base;
using Simulation.Game.Skills;
using Simulation.Spritesheet;
using Simulation.Util;

namespace Simulation.Game
{
    public class Player: LivingEntity
    {
        Simulation.Spritesheet.Spritesheet sheet;
        WalkingDirection curDirection = WalkingDirection.Idle;
        Animation curAnimation = null;

        private float velocity = 0.3f;
        private FireballSkill fireballSkill;

        public Player(): base(new Vector2(0, 0), new Rectangle(-8, -20, 16, 20))
        {
            fireballSkill = new FireballSkill(this, new Vector2(0, -20));
        }

        public void LoadContent()
        {
            Texture2D texture = SimulationGame.contentManager.Load<Texture2D>("player");
            sheet = new Simulation.Spritesheet.Spritesheet(texture).WithGrid((32, 48)).WithCellOrigin(new Point(16, 48)).WithFrameDuration(160);

            curAnimation = getAnimation(curDirection);
        }

        private int getAnimationOffset(WalkingDirection direction)
        {
            int offset = 0; /* 8 top 9 left 10 down 11 right */

            if ((direction & WalkingDirection.Left) == WalkingDirection.Left)
            {
                offset = 1;
            }
            else if ((direction & WalkingDirection.Right) == WalkingDirection.Right)
            {
                offset = 2;
            }
            else if ((direction & WalkingDirection.Up) == WalkingDirection.Up)
            {
                offset = 3;
            }

            return offset;
        }

        private Animation getAnimation(WalkingDirection direction)
        {
            int offset = getAnimationOffset(direction);

            // anim.Start(Repeat.Mode.Loop);

            return sheet.CreateAnimation((0, offset), (1, offset), (2, offset), (3, offset)); 
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
                SimulationGame.world.removeHitableObject(this);

                updatePosition(newPosition);
                
                SimulationGame.camera.Position = new Vector2(position.X, position.Y);
                SimulationGame.world.addHitableObject(this);
            }

            fireballSkill.Update(gameTime);
            curAnimation.Update(gameTime);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation, position, layerDepth: GeometryUtils.getLayerDepthFromPosition(position.X, position.Y));

            base.Draw(spriteBatch);
        }
    }
}
