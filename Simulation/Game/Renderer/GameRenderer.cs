using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer.Entities;

namespace Simulation.Game.Renderer
{
    public class GameRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, SimulationGame simulationGame)
        {
            simulationGame.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SimulationGame.camera, SpriteSortMode.FrontToBack);

            WorldRenderer.Draw(spriteBatch, gameTime);

            LivingEntityRenderer.Draw(spriteBatch, gameTime, SimulationGame.player);

            foreach (var effect in SimulationGame.effects)
            {
                effect.Draw(spriteBatch);
            }

            spriteBatch.End();

            // Debug
            spriteBatch.Draw(SimulationGame.camera.Debug);

            // Hud
            spriteBatch.Begin();

            SimulationGame.hud.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
