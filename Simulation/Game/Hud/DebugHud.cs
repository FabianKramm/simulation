using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util;

namespace Simulation.Game.Hud
{
    class DebugHud
    {
        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);
        private SpriteFont font;

        public void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            font = SimulationGame.contentManager.Load<SpriteFont>("Arial");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (SimulationGame.isDebug)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.resolution.Width, SimulationGame.resolution.Height), backgroundColor);

                Point currentBlock = GeometryUtils.getChunkPosition((int)SimulationGame.camera.Position.X, (int)SimulationGame.camera.Position.Y, World.World.BlockSize.X, World.World.BlockSize.Y);

                spriteBatch.DrawString(font, "Pos: " + SimulationGame.camera.Position.X + "," + SimulationGame.camera.Position.Y, new Vector2(SimulationGame.resolution.Width - 100, 20), Color.White);
                spriteBatch.DrawString(font, "Block: " + currentBlock.X + "," + currentBlock.Y, new Vector2(SimulationGame.resolution.Width - 100, 40), Color.White);
            }
        }
    }
}
