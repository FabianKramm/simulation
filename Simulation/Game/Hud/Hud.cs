using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public void LoadContent()
        {
            cursor = SimulationGame.contentManager.Load<Texture2D>(@"Misc\cursorDefault");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var position = Mouse.GetState().Position;

            SimulationGame.StringToDraw = position.X + "," + position.Y;

            spriteBatch.Draw(cursor, new Vector2(position.X, position.Y), scale: new Vector2(0.75f, 0.75f), color: Color.White);
        }
    }
}
