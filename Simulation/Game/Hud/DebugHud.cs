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

                Point currentBlock = GeometryUtils.getChunkPosition((int)SimulationGame.camera.Position.X, (int)SimulationGame.camera.Position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

                string currentPos = "Pos: " + SimulationGame.camera.Position.X + ", " + SimulationGame.camera.Position.Y;
                string currentBlockText = "Block: " + currentBlock.X + ", " + currentBlock.Y;
                string loadedChunks = "Loaded Chunks: " + SimulationGame.world.getLoadedChunkAmount() + " - " + SimulationGame.world.walkableGrid.getLoadedChunkAmount();

                spriteBatch.DrawString(font, currentPos, new Vector2(SimulationGame.resolution.Width - font.MeasureString(currentPos).X - 20, 20), Color.White);
                spriteBatch.DrawString(font, currentBlockText, new Vector2(SimulationGame.resolution.Width - font.MeasureString(currentBlockText).X - 20, 40), Color.White);
                spriteBatch.DrawString(font, loadedChunks, new Vector2(SimulationGame.resolution.Width - font.MeasureString(loadedChunks).X - 20, 60), Color.White);
            }
        }
    }
}
