using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.World;
using Comora;
using Simulation.Util;
using Simulation.Game.Hud;
using Simulation.Game.Renderer;
using Simulation.Game.Generator;
using Simulation.Game.Generator.Factories;
using System.Threading;
using Simulation.Util.Geometry;
using Simulation.Game.Objects.Entities;
using Simulation.Game.MetaData;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Simulation.Game.Serialization;
using System.Collections.Generic;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.MetaData.World;
using System.Diagnostics;
using System;

/*
 * Open Issues:
 * - Save World on exit
 * 
    Open Points:
    - World Generation
    - AI
*/

namespace Simulation
{
    public struct Size
    {
        public Size(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public int Width;
        public int Height;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SimulationGame: Microsoft.Xna.Framework.Game
    {
        public static readonly Size Resolution = new Size(1600, 768);
        public static readonly int TicksPerHour = 20;
        public static readonly int TicksPerDay = 24 * TicksPerHour;
        public static readonly int MilliSecondsPerTick = 500;

        public static SimulationGame I
        {
            get; private set;
        }

        public static MouseState MouseState
        {
            get; private set;
        }

        public static KeyboardState KeyboardState
        {
            get; private set;
        }

        public static bool IsPaused
        {
            get; private set;
        } = false;

        public static bool IsSurroundingEffectsOff
        {
            get; private set;
        } = false;

        public static bool IsConsoleOpen
        {
            get; private set;
        } = false;

        public static bool IsWorldBuilderOpen
        {
            get; private set;
        } = false;

        public static bool IsGodMode
        {
            get; private set;
        } = false;

        public static float Ticks
        {
            get; set;
        } = TicksPerHour * 12;

        public static bool IsDebug
        {
            get; private set;
        } = false;

        public static Camera Camera
        {
            get; private set;
        }

        public static WorldGrid World
        {
            get; private set;
        }

        public static WorldGenerator WorldGenerator
        {
            get; private set;
        }

        public static ContentManager ContentManager
        {
            get; private set;
        }

        public static Primitive PrimitiveDrawer
        {
            get; private set;
        }

        public static Hud Hud
        {
            get; private set;
        }
        
        public static Rect VisibleArea;

        public static Vector2 RealWorldMousePosition
        {
            get; private set;
        }

        public static Player Player
        {
            get; private set;
        }

        public static GraphicsDeviceManager Graphics
        {
            get; private set;
        }

        public static long DrawElapsedMillis
        {
            get; private set;
        }

        public static long UpdateElapsedMillis
        {
            get; private set;
        }

        private SpriteBatch spriteBatch;
        private float zoom = 1.0f;

        private bool pauseKeyDown = false;
        private bool godModeKeyDown = false;
        private bool debugKeyDown = false;
        private bool consoleKeyDown = false;
        private bool worldBuilderKeyDown = false;
        private bool surroundingEffectsKeyDown = false;

        public SimulationGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>((object sender, PreparingDeviceSettingsEventArgs eventargs) =>
            {
                eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            });

            Graphics.PreferredBackBufferWidth = Resolution.Width;  // set this value to the desired width of your window
            Graphics.PreferredBackBufferHeight = Resolution.Height;   // set this value to the desired height of your window
            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            ContentManager = Content;

            VisibleArea = Rect.Empty;
            
            IsDebug = false;

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            World = new WorldGrid();

            I = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Util.Util.CreateGameFolders();

            if(File.Exists(Util.Util.GetBlockTypesSavePath()))
                using (var stream = new StreamReader(Util.Util.GetBlockTypesSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    BlockType.lookup = (Dictionary<int,BlockType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), BlockType.lookup.GetType());
                }

            if (File.Exists(Util.Util.GetAmbientObjectTypesSavePath()))
                using (var stream = new StreamReader(Util.Util.GetAmbientObjectTypesSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    AmbientObjectType.lookup = (Dictionary<int, AmbientObjectType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), AmbientObjectType.lookup.GetType());
                }

            if (File.Exists(Util.Util.GetAmbientHitableObjectTypesSavePath()))
                using (var stream = new StreamReader(Util.Util.GetAmbientHitableObjectTypesSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    AmbientHitableObjectType.lookup = (Dictionary<int, AmbientHitableObjectType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), AmbientHitableObjectType.lookup.GetType());
                }

            /*if (File.Exists(Util.Util.GetLivingEntityTypesSavePath()))
                using (var stream = new StreamReader(Util.Util.GetLivingEntityTypesSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    LivingEntityType.lookup = (Dictionary<int, LivingEntityType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), LivingEntityType.lookup.GetType());
                }*/

            if (File.Exists(Util.Util.GetBiomeTypesSavePath()))
                using (var stream = new StreamReader(Util.Util.GetBiomeTypesSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    BiomeType.lookup = (Dictionary<int, BiomeType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), BiomeType.lookup.GetType());
                }

            if (File.Exists(Util.Util.GetPointOfInterestSavePath()))
                using (var stream = new StreamReader(Util.Util.GetPointOfInterestSavePath()))
                using (var reader = new JsonTextReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    PointOfInterestType.lookup = (Dictionary<int, PointOfInterestType>)SerializationUtils.Serializer.Deserialize(new JTokenReader(jToken), PointOfInterestType.lookup.GetType());
                }

            WorldGenerator = new WorldGenerator(1);

            // TODO: Add your initialization logic here
            Camera = new Camera(Graphics.GraphicsDevice);
            Camera.Zoom = zoom;

            Player = (Player)LivingEntityType.Create(new WorldPosition(0, 0, Interior.Outside), LivingEntityType.lookup[LivingEntityType.Player]);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Util.UI.Elements.Button.ButtonFont = ContentManager.Load<SpriteFont>("Arial");
            Util.UI.Elements.TextView.TextViewFont = ContentManager.Load<SpriteFont>("ArialSmall");

            PrimitiveDrawer = new Primitive(Graphics.GraphicsDevice, spriteBatch);
            PrimitiveDrawer.Depth = 1.0f;

            Hud = new Hud();

            Camera.LoadContent();
            Hud.LoadContent();

            MovingEntityRenderer.LoadContent();
            GameRenderer.LoadContent();

            Player.Init();
            Player.ConnectToWorld();

            LivingEntityType.Create(new WorldPosition(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3, Interior.Outside), LivingEntityType.lookup[3]).ConnectToWorld();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler((object sender, EventArgs e) =>
            {
                SaveAndExit();
            });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void SaveAndExit()
        {
            IsPaused = true;

            World.SaveAll();
            World.WalkableGrid.SaveAll();
            World.InteriorManager.SaveAll();
            
            using (var stream = new StreamWriter(Util.Util.GetBlockTypesSavePath()))
            using (var writer = new JsonTextWriter(stream))
            {
                writer.Formatting = Formatting.Indented;

                JToken.FromObject(BlockType.lookup, SerializationUtils.Serializer).WriteTo(writer);
            }

            using (var stream = new StreamWriter(Util.Util.GetAmbientObjectTypesSavePath()))
            using (var writer = new JsonTextWriter(stream))
            {
                writer.Formatting = Formatting.Indented;

                JToken.FromObject(AmbientObjectType.lookup, SerializationUtils.Serializer).WriteTo(writer);
            }

            using (var stream = new StreamWriter(Util.Util.GetAmbientHitableObjectTypesSavePath()))
            using (var writer = new JsonTextWriter(stream))
            {
                writer.Formatting = Formatting.Indented;

                JToken.FromObject(AmbientHitableObjectType.lookup, SerializationUtils.Serializer).WriteTo(writer);
            }

            /*using (var stream = new StreamWriter(Util.Util.GetLivingEntityTypesSavePath()))
            using (var writer = new JsonTextWriter(stream))
            {
                writer.Formatting = Formatting.Indented;

                JToken.FromObject(LivingEntityType.lookup, SerializationUtils.Serializer).WriteTo(writer);
            }*/

            Exit();
        }

        private void updateVisibleArea()
        {
            VisibleArea.X = (int)(SimulationGame.Camera.Position.X - Resolution.Width * 0.5f * (1/zoom)) - WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.X;
            VisibleArea.Y = (int)(SimulationGame.Camera.Position.Y - Resolution.Height * 0.5f * (1/zoom)) - WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.Y;

            VisibleArea.Width = (int)(Resolution.Width * (1 / zoom) + 2 * WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.X);
            VisibleArea.Height = (int)(Resolution.Height * (1 / zoom) + 2 * WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.Y);
        }

        private void updateMousePosition()
        {
            var _mousePosition = MouseState.Position;

            RealWorldMousePosition = new Vector2((Camera.Position.X - Resolution.Width * 0.5f) + _mousePosition.X, (Camera.Position.Y - Resolution.Height * 0.5f) + _mousePosition.Y);
        }

        public static Vector2 ConvertWorldPositionToUIPosition(float worldPositionX, float worldPositionY)
        {
            return new Vector2(worldPositionX - (Camera.Position.X - Resolution.Width * 0.5f), worldPositionY - (Camera.Position.Y - Resolution.Height * 0.5f));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var stopwatch = Stopwatch.StartNew();

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            if (KeyboardState.IsKeyDown(Keys.Escape) && (KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift)))
                SaveAndExit();

            if (KeyboardState.IsKeyDown(Keys.F1))
            {
                if (!debugKeyDown)
                {
                    debugKeyDown = true;
                    IsDebug = !IsDebug;
                }
            }
            else
            {
                debugKeyDown = false;
            }

            if (KeyboardState.IsKeyDown(Keys.F2))
            {
                if (!worldBuilderKeyDown)
                {
                    worldBuilderKeyDown = true;
                    IsWorldBuilderOpen = !IsWorldBuilderOpen;
                }
            }
            else
            {
                worldBuilderKeyDown = false;
            }

            if (KeyboardState.IsKeyDown(Keys.F3))
            {
                if (!pauseKeyDown)
                {
                    pauseKeyDown = true;
                    IsPaused = !IsPaused;
                }
            }
            else
            {
                pauseKeyDown = false;
            }

            if (KeyboardState.IsKeyDown(Keys.F4))
            {
                if (!consoleKeyDown)
                {
                    consoleKeyDown = true;
                    IsConsoleOpen = !IsConsoleOpen;
                }
            }
            else
            {
                consoleKeyDown = false;
            }

            if (KeyboardState.IsKeyDown(Keys.F5))
            {
                if (!godModeKeyDown)
                {
                    godModeKeyDown = true;

                    if(IsGodMode)
                    {
                        Player.ConnectToWorld();
                        Player.Velocity -= 0.3f;
                    }
                    else if(!IsGodMode)
                    {
                        Player.DisconnectFromWorld();
                        Player.Velocity += 0.3f;
                    }

                    IsGodMode = !IsGodMode;
                }
            }
            else
            {
                godModeKeyDown = false;
            }

            if (KeyboardState.IsKeyDown(Keys.F6))
            {
                if (!surroundingEffectsKeyDown)
                {
                    surroundingEffectsKeyDown = true;

                    IsSurroundingEffectsOff = !IsSurroundingEffectsOff;
                }
            }
            else
            {
                surroundingEffectsKeyDown = false;
            }

            Hud.Update(gameTime);

            if (IsPaused == false)
            {
                Ticks += (float)gameTime.ElapsedGameTime.TotalMilliseconds / (float)MilliSecondsPerTick;

                Camera.Update(gameTime);
                World.Update(gameTime);
            }

            base.Update(gameTime);

            stopwatch.Stop();
            UpdateElapsedMillis = stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var stopwatch = Stopwatch.StartNew();

            updateVisibleArea();
            updateMousePosition();

            GameRenderer.Draw(spriteBatch, gameTime, this);

            base.Draw(gameTime);

            stopwatch.Stop();
            DrawElapsedMillis = stopwatch.ElapsedMilliseconds;
        }
    }
}
