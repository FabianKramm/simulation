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
using Simulation.Game.World.Generator;
using System.IO;

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
        public static int TicksPerHour = 20;
        public static int TicksPerDay = 24 * TicksPerHour;
        private static int MilliSecondsPerTick = 500;

        public static float Ticks
        {
            get; private set;
        }

        public static bool isDebug
        {
            get; private set;
        }

        public static Camera camera
        {
            get; private set;
        }

        public static WorldGrid world
        {
            get; private set;
        }

        public static WorldGenerator worldGenerator;
        //{
        //    get; private set;
        //}

        public static ContentManager contentManager
        {
            get; private set;
        }

        public static Primitive primitiveDrawer
        {
            get; private set;
        }

        public static Hud hud
        {
            get; private set;
        }

        public static Size resolution = new Size(1280, 768);
        public static Rectangle visibleArea;

        public static Vector2 mousePosition
        {
            get; private set;
        }

        public static List<Simulation.Game.Effects.Effect> effects = new List<Simulation.Game.Effects.Effect>();
        public static Player player;

        public static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private float zoom = 1.0f;

        private bool debugKeyDown = false;

        public SimulationGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = resolution.Width;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = resolution.Height;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            contentManager = Content;

            world = new WorldGrid();
            hud = new Hud();

            visibleArea = Rectangle.Empty;
            
            isDebug = false;
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

            worldGenerator = new WorldGenerator(1);

            // TODO: Add your initialization logic here
            camera = new Camera(graphics.GraphicsDevice);
            camera.Zoom = zoom;

            player = new Player();

            world.addDurableEntity(player);

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

            primitiveDrawer = new Primitive(graphics.GraphicsDevice, spriteBatch);
            primitiveDrawer.Depth = 1.0f;

            camera.LoadContent();
            hud.LoadContent();

            GameRenderer.LoadContent();
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
            visibleArea.X = (int)(SimulationGame.camera.Position.X - resolution.Width * 0.5f * (1/zoom)) - WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.X;
            visibleArea.Y = (int)(SimulationGame.camera.Position.Y - resolution.Height * 0.5f * (1/zoom)) - WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.Y;

            visibleArea.Width = (int)(resolution.Width * (1 / zoom) + 2 * WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.X);
            visibleArea.Height = (int)(resolution.Height * (1 / zoom) + 2 * WorldGrid.RenderOuterBlockRange * WorldGrid.BlockSize.Y);
        }

        private void updateMousePosition()
        {
            var _mousePosition = Mouse.GetState().Position;

            mousePosition = new Vector2((SimulationGame.camera.Position.X - resolution.Width * 0.5f) + _mousePosition.X, (SimulationGame.camera.Position.Y - resolution.Height * 0.5f) + _mousePosition.Y);
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
                    SimulationGame.isDebug = !SimulationGame.isDebug;
                }
            }
            else
            {
                debugKeyDown = false;
            }

            player.Update(gameTime);
            camera.Update(gameTime);

            for(int i=0;i<effects.Count;i++)
            {
                var effect = effects[i];

                effect.Update(gameTime);

                if(effect.isFinished)
                {
                    effects.Remove(effect);
                    i--;
                }
            }

            hud.Update(gameTime);
            world.Update(gameTime);

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
