using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer.Effects;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class GameRenderer
    {
        

        public static void LoadContent()
        {
            LightningRenderer.LoadContent();
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, SimulationGame simulationGame)
        {
            simulationGame.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SimulationGame.Camera, SpriteSortMode.FrontToBack);

            WorldRenderer.Draw(spriteBatch, gameTime);

            if (SimulationGame.IsGodMode)
            {
               MovingEntityRenderer.Draw(spriteBatch, gameTime, SimulationGame.Player);
            }

            spriteBatch.End();

            LightningRenderer.Draw(spriteBatch, gameTime);

            // Hud
            spriteBatch.Begin();

            SimulationGame.Hud.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }
    }
}
