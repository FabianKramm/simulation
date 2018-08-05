using Microsoft.CSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Simulation.Game.Hud
{
    class DebugHud
    {
        private static readonly int consoleLines = 11;

        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);
        private Color consoleColor = new Color(Color.Black, 0.4f);

        private SpriteFont font;

        private string command = "";

        private Dictionary<int, bool> keysPressed = new Dictionary<int, bool>();
        private TimeSpan lastBackKeyPress = TimeSpan.Zero;

        private static List<string> consoleOutput = new List<string>();
        
        public void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            font = SimulationGame.ContentManager.Load<SpriteFont>("Arial");
        }

        public static void ConsoleWrite(string message)
        {
            message = Regex.Replace(message, "[^A-Za-z0-9\\\"\\s]", "");

            consoleOutput.Add(message);

            if (consoleOutput.Count >= consoleLines)
            {
                consoleOutput.RemoveAt(0);
            }
        }

        // Eval > Evaluates C# sourcelanguage
        public void Eval(string code)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var p = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, null, true);

            p.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
            p.GenerateInMemory = true;
            p.GenerateExecutable = false;

            CompilerResults r = csc.CompileAssemblyFromSource(p, "using System; using Simulation.Game.Hud; class p {public static void c(){ConsoleFunctions." + code + ";}}");

            if (r.Errors.Count > 0)
            {
                foreach (var error in r.Errors)
                    ConsoleWrite(((CompilerError)error).ErrorText);

                return;
            }

            var a = r.CompiledAssembly;
            MethodInfo o = a.CreateInstance("p").GetType().GetMethod("c");

            o.Invoke(o, null);
        }

        private void executeCommand()
        {
            Eval(command);

            command = "";
        }

        public void Update(GameTime gameTime)
        {
            if(SimulationGame.IsConsoleOpen)
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
                            else if(i == (int)Keys.Enter)
                            {
                                executeCommand();
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
            if (SimulationGame.IsDebug)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);

                Point currentBlock = GeometryUtils.GetChunkPosition((int)SimulationGame.Camera.Position.X, (int)SimulationGame.Camera.Position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

                string time = "Time: " + (TimeUtils.GetCurrentDayTick() / SimulationGame.TicksPerHour) + ":" + (TimeUtils.GetCurrentDayTick() % SimulationGame.TicksPerHour);
                string currentPos = "Pos: " + SimulationGame.Camera.Position.X + ", " + SimulationGame.Camera.Position.Y;
                string currentBlockText = "Block: " + currentBlock.X + ", " + currentBlock.Y;
                string loadedChunks = "Chunks: " + SimulationGame.World.CountLoaded() + " World, " + SimulationGame.World.WalkableGrid.CountLoaded() + " Walk, " + SimulationGame.World.InteriorManager.CountLoaded() + " Ints";
                string interiorID = "InteriorID: " + SimulationGame.Player.InteriorID;

                spriteBatch.DrawString(font, time, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(time).X - 20, 20), Color.White);
                spriteBatch.DrawString(font, currentPos, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(currentPos).X - 20, 40), Color.White);
                spriteBatch.DrawString(font, currentBlockText, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(currentBlockText).X - 20, 60), Color.White);
                spriteBatch.DrawString(font, loadedChunks, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(loadedChunks).X - 20, 80), Color.White);

                if(SimulationGame.Player.InteriorID != Interior.Outside)
                {
                    spriteBatch.DrawString(font, interiorID, new Vector2(SimulationGame.Resolution.Width - font.MeasureString(interiorID).X - 20, 100), Color.White);
                }
            }

            if(SimulationGame.IsConsoleOpen)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(SimulationGame.Resolution.Width - 510, SimulationGame.Resolution.Height - 210, 490, 190), consoleColor);
                spriteBatch.DrawString(font, "> " + command, new Vector2(SimulationGame.Resolution.Width - 500, SimulationGame.Resolution.Height - 200), Color.White);

                for (int i = 0; i < consoleOutput.Count; i++)
                {
                    var message = consoleOutput[i];

                    spriteBatch.DrawString(font, message, new Vector2(SimulationGame.Resolution.Width - 500, SimulationGame.Resolution.Height - 200 + 16 + i * 16), Color.White);
                }
            }
        }
    }
}
