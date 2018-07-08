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
            backgroundOverlay = new Texture2D(SimulationGame.graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            font = SimulationGame.contentManager.Load<SpriteFont>("Arial");
        }

        public void Update(GameTime gameTime)
        {
            if(SimulationGame.isDebug)
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
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (SimulationGame.isDebug)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.resolution.Width, SimulationGame.resolution.Height), backgroundColor);
                spriteBatch.Draw(backgroundOverlay, new Rectangle(SimulationGame.resolution.Width - 510, SimulationGame.resolution.Height - 210, 490, 190), consoleColor);


                Point currentBlock = GeometryUtils.getChunkPosition((int)SimulationGame.camera.Position.X, (int)SimulationGame.camera.Position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

                string currentPos = "Pos: " + SimulationGame.camera.Position.X + ", " + SimulationGame.camera.Position.Y;
                string currentBlockText = "Block: " + currentBlock.X + ", " + currentBlock.Y;
                string loadedChunks = "Loaded Chunks: " + SimulationGame.world.getLoadedChunkAmount() + " - " + SimulationGame.world.walkableGrid.getLoadedChunkAmount();

                spriteBatch.DrawString(font, currentPos, new Vector2(SimulationGame.resolution.Width - font.MeasureString(currentPos).X - 20, 20), Color.White);
                spriteBatch.DrawString(font, currentBlockText, new Vector2(SimulationGame.resolution.Width - font.MeasureString(currentBlockText).X - 20, 40), Color.White);
                spriteBatch.DrawString(font, loadedChunks, new Vector2(SimulationGame.resolution.Width - font.MeasureString(loadedChunks).X - 20, 60), Color.White);

                spriteBatch.DrawString(font, "> " + command, new Vector2(SimulationGame.resolution.Width - 500, SimulationGame.resolution.Height - 200), Color.White);
            }
        }
    }
}
