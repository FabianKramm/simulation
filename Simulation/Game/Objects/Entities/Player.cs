using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.Skills;
using Simulation.Game.World;
using Simulation.Game.Enums;
using Simulation.Util.Geometry;
using Simulation.Util.Collision;
using System.Collections.Generic;
using System.Diagnostics;
using Simulation.Game.Hud;
using System;

namespace Simulation.Game.Objects.Entities
{
    public class Player: DurableEntity
    {
        private bool leftMouseClick;

        public Player(): base(LivingEntityType.PLAYER, new WorldPosition(0, 0, Interior.Outside), FractionType.PLAYER, 3)
        {
            Skills = new Skill[]
            {
                new FireballSkill(this, new Vector2(0, -10)),
                new SlashSkill(this, new Vector2(0, -14))
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
                // Skills[1].Use(SimulationGame.MousePosition);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!leftMouseClick)
                {
                    List<LivingEntity> entities = CollisionUtils.GetLivingHittedObjects(new Rect((int)SimulationGame.MousePosition.X, (int)SimulationGame.MousePosition.Y, 1, 1), InteriorID, this);

                    if(entities.Count > 0)
                    {
                        var target = entities[0];

                        var stopwatch = Stopwatch.StartNew();
                        stopwatch.Start();

                        var isBlocked = CollisionUtils.IsSightBlocked(this, target, 16);

                        stopwatch.Stop();

                        GameConsole.WriteLine("IsBlocked to Target(" + target.ID + "): " + (isBlocked ? "true" : "false") + " took " + stopwatch.ElapsedMilliseconds + "ms");
                    }

                    // Point clickedBlock = GeometryUtils.GetChunkPosition((int)SimulationGame.MousePosition.X, (int)SimulationGame.MousePosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                    
                    // WalkToBlock(new WorldPosition(clickedBlock.X, clickedBlock.Y, SimulationGame.Player.InteriorID));
                    // WalkToPosition(new WorldPosition(SimulationGame.MousePosition.X, SimulationGame.MousePosition.Y, SimulationGame.Player.InteriorID));

                    leftMouseClick = true;
                }
            }
            else
            {
                leftMouseClick = false;
            }

            Direction = newDirection;

            base.Update(gameTime);
        }
    }
}
