using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Hud
{
    public class Hud
    {
        private Texture2D cursor;
        
        private GameConsole console;
        private DebugHud debugHud;
        private WorldBuilder.WorldBuilder worldBuilder;
        
        public void LoadContent()
        {
            console = new GameConsole();
            console.LoadContent();

            cursor = SimulationGame.ContentManager.Load<Texture2D>(@"Misc\cursorDefault");

            debugHud = new DebugHud();
            debugHud.LoadContent();

            worldBuilder = new WorldBuilder.WorldBuilder();
            worldBuilder.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            debugHud.Update(gameTime);
            worldBuilder.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var position = Mouse.GetState().Position;

            debugHud.Draw(spriteBatch, gameTime);

            // SimulationGame.StringToDraw = position.X + "," + position.Y;

            console.Draw(spriteBatch);
            worldBuilder.Draw(spriteBatch, gameTime);

            spriteBatch.Draw(cursor, new Vector2(position.X, position.Y), null, Color.White, 0.0f, Vector2.Zero, new Vector2(0.75f, 0.75f), SpriteEffects.None, 1.0f);
        }
    }
}
