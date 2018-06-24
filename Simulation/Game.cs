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
        public static bool isDebug
        {
            get; private set;
        }

        public static Camera camera
        {
            get; private set;
        }

        public static World world
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

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private float zoom = 1.0f;

        public SimulationGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = resolution.Width;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = resolution.Height;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            contentManager = Content;

            world = new World();
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

            player.LoadContent();
            camera.LoadContent();
            hud.LoadContent();
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
            visibleArea.X = (int)(SimulationGame.camera.Position.X - resolution.Width * 0.5f * (1/zoom)) - World.RenderOuterBlockRange * World.BlockSize.X;
            visibleArea.Y = (int)(SimulationGame.camera.Position.Y - resolution.Height * 0.5f * (1/zoom)) - World.RenderOuterBlockRange * World.BlockSize.Y;

            visibleArea.Width = (int)(resolution.Width * (1 / zoom) + 2 * World.RenderOuterBlockRange * World.BlockSize.X);
            visibleArea.Height = (int)(resolution.Height * (1 / zoom) + 2 * World.RenderOuterBlockRange * World.BlockSize.Y);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            player.Update(gameTime);
            camera.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                isDebug = true;
                camera.Debug.IsVisible = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                isDebug = false;
                camera.Debug.IsVisible = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                camera.Debug.Grid.AddLines(32, Color.White, 2);
            }

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

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(camera, SpriteSortMode.FrontToBack);

            WorldRenderer.Draw(spriteBatch);
            player.Draw(spriteBatch);

            foreach (var effect in effects)
            {
                effect.Draw(spriteBatch);
            }

            spriteBatch.End();

            // Debug
            spriteBatch.Draw(camera.Debug);

            // Hud
            spriteBatch.Begin();
            hud.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
