using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.Skills;
using Simulation.Game.World;
using Simulation.Game.Enums;

namespace Simulation.Game.Objects.Entities
{
    public class Player: DurableEntity
    {
        private bool leftMouseClick;

        public Player(): base(LivingEntityType.PLAYER, new WorldPosition(0, 0), FractionType.PLAYER, 3)
        {
            Skills = new Skill[]
            {
                new FireballSkill(this, new Vector2(0, -20)),
                new SlashSkill(this, new Vector2(0, -24))
            };

            Velocity = 0.2f;
        }

        protected override void UpdatePosition(WorldPosition newPosition)
        {
            base.UpdatePosition(newPosition);

            SimulationGame.Camera.Position = new Vector2((int)Position.X, (int)Position.Y);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            Vector2 newDirection = Vector2.Zero;

            if (state.IsKeyDown(Keys.D))
            {
                newDirection.X += 1.0f;
            }

            if (state.IsKeyDown(Keys.A))
            {
                newDirection.X -= 1.0f;
            }

            if (state.IsKeyDown(Keys.W))
            {
                newDirection.Y -= 1.0f;
            }

            if (state.IsKeyDown(Keys.S))
            {
                newDirection.Y += 1.0f;
            }

            if (state.IsKeyDown(Keys.D1))
            {
                Skills[0].Use(SimulationGame.MousePosition);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Skills[1].Use(SimulationGame.MousePosition);
            }

            /* if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // slashSkill.use(SimulationGame.MousePosition);

                if (!leftMouseClick)
                {
                    // Point clickedBlock = GeometryUtils.GetChunkPosition((int)SimulationGame.MousePosition.X, (int)SimulationGame.MousePosition.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);
                    //WalkToBlock(new WorldPosition(clickedBlock.X, clickedBlock.Y, SimulationGame.Player.InteriorID));

                    WalkToPosition(new WorldPosition(SimulationGame.MousePosition.X, SimulationGame.MousePosition.Y, SimulationGame.Player.InteriorID));

                    leftMouseClick = true;
                }
            }
            else
            {
                leftMouseClick = false;
            } */

            Direction = newDirection;

            base.Update(gameTime);
        }
    }
}
