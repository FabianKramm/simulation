using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Hud
{
    class DebugHud
    {
        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);
        private Color consoleColor = new Color(Color.Black, 0.4f);

        private SpriteFont font;

        private string command = "";

        private Dictionary<int, bool> keysPressed = new Dictionary<int, bool>();
        private TimeSpan lastBackKeyPress = TimeSpan.Zero;

        public void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            font = SimulationGame.ContentManager.Load<SpriteFont>("Arial");
        }

        public void Update(GameTime gameTime)
        {
            /* if(SimulationGame.isDebug)
            {
                KeyboardState state = Keyboard.GetState();
                bool shiftDown = state.CapsLock || state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

                for (int i = 0; i <= 255; i++)
                {
                    if (state.IsKeyDown((Keys)i))
                    {
                        if (!keysPressed.ContainsKey(i))
                        {
                            if(i == (int)Keys.Back)
                            {
                                if (!keysPressed.ContainsKey((int)Keys.Back))
                                {
                                    if (command.Length > 0)
                                    {
                                        command = command.Remove(command.Length - 1);
                                    }

                                    lastBackKeyPress = TimeSpan.Zero;
                                    keysPressed[(int)Keys.Back] = true;
                                }
                                else
                                {
                                    lastBackKeyPress += gameTime.ElapsedGameTime;

                                    if (lastBackKeyPress >= TimeSpan.FromMilliseconds(150))
                                    {
                                        keysPressed.Remove((int)Keys.Back);
                                    }
                                }
                            }
                            else
                            {
                                char commandChar;

                                if (KeyboardUtils.KeyToString((Keys)i, shiftDown, out commandChar))
                                {
                                    command += commandChar;
                                }
                            }
                            
                            keysPressed[i] = true;
                        }
                    }
                    else
                    {
                        keysPressed.Remove(i);
                    }
                }
            } */
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (SimulationGame.IsDebug)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);
                spriteBatch.Draw(backgroundOverlay, new Rectangle(SimulationGame.Resolution.Width - 510, SimulationGame.Resolution.Height - 210, 490, 190), consoleColor);


                Point currentBlock = GeometryUtils.getChunkPosition((int)SimulationGame.Camera.Position.X, (int)SimulationGame.Camera.Position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

                string time = "Time: " + (TimeUtils.GetCurrentDayTick() / SimulationGame.TicksPerHour) + ":" + (TimeUtils.GetCurrentDayTick() % SimulationGame.TicksPerHour);
                string currentPos = "Pos: " + SimulationGame.Camera.Position.X + ", " + SimulationGame.Camera.Position.Y;
                string currentBlockText = "Block: " + currentBlock.X + ", " + currentBlock.Y;
                string loadedChunks = "Loaded Chunks: " + SimulationGame.World.getLoadedChunkAmount() + " - " + SimulationGame.World.walkableGrid.getLoadedChunkAmount();

                spriteBatch.DrawString(font, time, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(time).X - 20, 20), Color.White);
                spriteBatch.DrawString(font, currentPos, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(currentPos).X - 20, 40), Color.White);
                spriteBatch.DrawString(font, currentBlockText, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(currentBlockText).X - 20, 60), Color.White);
                spriteBatch.DrawString(font, loadedChunks, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(loadedChunks).X - 20, 80), Color.White);

                spriteBatch.DrawString(font, "> " + command, new Vector2(SimulationGame.Resolution.Width - 500, SimulationGame.Resolution.Height - 200), Color.White);
            }
        }
    }
}
