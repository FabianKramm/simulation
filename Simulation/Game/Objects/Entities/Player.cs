using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.World;

namespace Simulation.Game.Objects.Entities
{
    public class Player: DurableEntity
    {
        private bool leftMouseClick;

        // Create from JSON
        protected Player() : base() { }

        public Player(WorldPosition worldPosition): base(worldPosition) { }

        public override void UpdatePosition(WorldPosition newPosition)
        {
            base.UpdatePosition(newPosition);

            SimulationGame.Camera.Position = new Vector2((int)Position.X, (int)Position.Y);
        }

        public override void Update(GameTime gameTime)
        {
            if(!SimulationGame.IsConsoleOpen)
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
                    Skills[2].Use(SimulationGame.RealWorldMousePosition);
                }

                if (state.IsKeyDown(Keys.D2))
                {
                    Skills[1].Use(SimulationGame.RealWorldMousePosition);
                }

                if(!SimulationGame.IsWorldBuilderOpen)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Skills[0].Use(SimulationGame.RealWorldMousePosition);
                    }

                    /*
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (!leftMouseClick)
                        {
                            Talk("Test");

                            // Point clickedBlock = GeometryUtils.GetChunkPosition((int)SimulationGame.MousePosition.X, (int)SimulationGame.MousePosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                            // WalkToBlock(new WorldPosition(clickedBlock.X, clickedBlock.Y, SimulationGame.Player.InteriorID));
                            // WalkToPosition(new WorldPosition(SimulationGame.MousePosition.X, SimulationGame.MousePosition.Y, SimulationGame.Player.InteriorID));

                            leftMouseClick = true;
                        }
                    }
                    else
                    {
                        leftMouseClick = false;
                    }*/
                }

                SetDirection(newDirection);

                base.Update(gameTime);
            }
        }
    }
}
