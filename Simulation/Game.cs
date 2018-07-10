using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game;
using Simulation.Game.World;
using Simulation.Spritesheet;
using Comora;
using System.Collections.Generic;
using Simulation.Util;
using Simulation.Game.Hud;
using Simulation.Game.Renderer;
using System.IO;
using Simulation.Game.Generator;
using Simulation.Game.Factories;
using System.Threading;

/*
    Open Points:
    - World Generation
    - Pathfinding
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
    public class SimulationGame : Microsoft.Xna.Framework.Game
    {
        public static readonly int TicksPerHour = 20;
        public static readonly int TicksPerDay = 24 * TicksPerHour;
        public static readonly int MilliSecondsPerTick = 500;

        public static float Ticks
        {
            get; private set;
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

        public static readonly Size Resolution = new Size(1280, 768);
        public static Rectangle VisibleArea;

        public static Vector2 MousePosition
        {
            get; private set;
        }

        public static List<Simulation.Game.Effects.Effect> effects = new List<Simulation.Game.Effects.Effect>();
        public static Player Player;

        public static GraphicsDeviceManager Graphics;

        private SpriteBatch spriteBatch;
        private float zoom = 1.0f;
        private bool debugKeyDown = false;

        public SimulationGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = Resolution.Width;  // set this value to the desired width of your window
            Graphics.PreferredBackBufferHeight = Resolution.Height;   // set this value to the desired height of your window
            Graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            ContentManager = Content;

            World = new WorldGrid();
            Hud = new Hud();

            VisibleArea = Rectangle.Empty;
            
            IsDebug = false;

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Util.Util.createGameFolders();

            WorldGenerator = new WorldGenerator(1);

            // TODO: Add your initialization logic here
            Camera = new Camera(Graphics.GraphicsDevice);
            Camera.Zoom = zoom;

            Player = new Player();

            World.addDurableEntity(Player);

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

            PrimitiveDrawer = new Primitive(Graphics.GraphicsDevice, spriteBatch);
            PrimitiveDrawer.Depth = 1.0f;

            Camera.LoadContent();
            Hud.LoadContent();

            GameRenderer.LoadContent();

            var Geralt = DurableEntityFactory.CreateGeralt();

            SimulationGame.World.addDurableEntity(Geralt);
            WorldGridChunk.GetWorldGridChunk(WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y).AddContainedObject(Geralt);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            var _mousePosition = Mouse.GetState().Position;

            MousePosition = new Vector2((SimulationGame.Camera.Position.X - Resolution.Width * 0.5f) + _mousePosition.X, (SimulationGame.Camera.Position.Y - Resolution.Height * 0.5f) + _mousePosition.Y);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Ticks += (float)gameTime.ElapsedGameTime.Milliseconds / (float)MilliSecondsPerTick;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if(!debugKeyDown)
                {
                    debugKeyDown = true;
                    SimulationGame.IsDebug = !SimulationGame.IsDebug;
                }
            }
            else
            {
                debugKeyDown = false;
            }

            Player.Update(gameTime);
            Camera.Update(gameTime);

            for(int i=0;i<effects.Count;i++)
            {
                var effect = effects[i];

                effect.Update(gameTime);

                if(effect.IsFinished)
                {
                    effects.Remove(effect);
                    i--;
                }
            }

            Hud.Update(gameTime);
            World.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            updateVisibleArea();
            updateMousePosition();

            GameRenderer.Draw(spriteBatch, gameTime, this);

            base.Draw(gameTime);
        }
    }
}
