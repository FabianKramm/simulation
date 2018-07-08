using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer.Entities;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class GameRenderer
    {
        private static Effect nightEffect;

        private static Color dayColor = new Color(255, 240, 240, 255); //

        private static Color startNightColor = new Color(100, 100, 200, 255); // 20 - 5 o'Clock
        private static Color midNightColor = new Color(80, 80, 180, 255); // 20 - 5 o'Clock

        private static Color dawnColor = new Color(230, 160, 160, 255); // 5 - 8 o'Clock
        private static Color duskColor = new Color(230, 160, 160, 255); // 18 - 20 o'Clock, 

        private static List<(int, Color)> dayNightCycleColors = new List<(int, Color)>
        {
            (0 * SimulationGame.TicksPerHour, new Color(80, 80, 180, 255)),
            (5 * SimulationGame.TicksPerHour, new Color(100, 100, 200, 255)),
            (7 * SimulationGame.TicksPerHour, new Color(220, 130, 130, 255)),
            (9 * SimulationGame.TicksPerHour, new Color(235, 220, 220, 255)),
            (14 * SimulationGame.TicksPerHour, new Color(255, 240, 240, 255)),
            (18 * SimulationGame.TicksPerHour, new Color(235, 220, 220, 255)),
            (20 * SimulationGame.TicksPerHour, new Color(220, 130, 130, 255)),
            (22 * SimulationGame.TicksPerHour, new Color(100, 100, 200, 255))
        };

        public static Color BlendColor;

        public static void LoadContent()
        {
            nightEffect = SimulationGame.contentManager.Load<Effect>("Effects/Night");
        }

        private static void setBlendColor()
        {
            int currentDayTick = TimeUtils.GetCurrentDayTick();

            int lowerBound = 0;
            int upperBound = 0;
            Color from = Color.White;
            Color to = Color.White;

            for (int i = 0; i < dayNightCycleColors.Count; i++)
            {
                if(currentDayTick < dayNightCycleColors[i].Item1 || i == dayNightCycleColors.Count - 1)
                {
                    if(i == dayNightCycleColors.Count - 1 && currentDayTick >= dayNightCycleColors[i].Item1)
                    {
                        from = dayNightCycleColors[i].Item2;
                        to = dayNightCycleColors[0].Item2;

                        lowerBound = dayNightCycleColors[i].Item1;
                        upperBound = 24 * SimulationGame.TicksPerHour;
                    }
                    else
                    {
                        from = dayNightCycleColors[i - 1].Item2;
                        to = dayNightCycleColors[i].Item2;

                        lowerBound = dayNightCycleColors[i - 1].Item1;
                        upperBound = dayNightCycleColors[i].Item1;
                    }

                    break;
                }
            }

            float distance = (float)(currentDayTick - lowerBound) / (float)(upperBound - lowerBound);

            BlendColor = new Color(
                from.R + (int)(distance * (to.R - from.R)),
                from.G + (int)(distance * (to.G - from.G)),
                from.B + (int)(distance * (to.B - from.B)),
                255
            );
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, SimulationGame simulationGame)
        {
            simulationGame.GraphicsDevice.Clear(Color.Black);
            setBlendColor();

            spriteBatch.Begin(SimulationGame.camera, SpriteSortMode.FrontToBack);

            WorldRenderer.Draw(spriteBatch, gameTime);

            LivingEntityRenderer.Draw(spriteBatch, gameTime, SimulationGame.player);

            foreach (var effect in SimulationGame.effects)
            {
                effect.Draw(spriteBatch);
            }

            spriteBatch.End();

            // Debug
            // spriteBatch.Draw(SimulationGame.camera.Debug);

            // Hud
            spriteBatch.Begin();

            SimulationGame.hud.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }
    }
}
