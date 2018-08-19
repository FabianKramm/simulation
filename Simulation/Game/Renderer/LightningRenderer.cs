using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public static class LightningRenderer
    {
        private struct LightningPosition
        {
            public Lightning Lightning;
            public Vector2 Position;
        }

        private static RenderTarget2D renderTarget;
        private static Texture2D softLightning;
        private static Texture2D inverseSoftLightning;
        private static Texture2D backgroundOverlay;
        private static List<LightningPosition> lightnings = new List<LightningPosition>();
        private static List<(int, Color)> dayNightCycleColors = new List<(int, Color)>()
        {
            (0 * SimulationGame.TicksPerHour, new Color(160, 160, 225, 255)),
            (5 * SimulationGame.TicksPerHour, new Color(180, 180, 230, 255)),
            (7 * SimulationGame.TicksPerHour, new Color(220, 200, 200, 255)),
            (9 * SimulationGame.TicksPerHour, new Color(235, 235, 235, 255)),
            (14 * SimulationGame.TicksPerHour, new Color(245, 245, 245, 255)),
            (18 * SimulationGame.TicksPerHour, new Color(235, 235, 235, 255)),
            (20 * SimulationGame.TicksPerHour, new Color(240, 200, 200, 255)),
            (22 * SimulationGame.TicksPerHour, new Color(180, 180, 225, 255))
        };

        private static List<(int, int)> dayNightCycleDarkness = new List<(int, int)>()
        {
            (0 * SimulationGame.TicksPerHour, 180),
            (5 * SimulationGame.TicksPerHour, 160),
            (7 * SimulationGame.TicksPerHour, 100),
            (9 * SimulationGame.TicksPerHour, 20),
            (14 * SimulationGame.TicksPerHour, 0),
            (18 * SimulationGame.TicksPerHour, 20),
            (20 * SimulationGame.TicksPerHour, 100),
            (22 * SimulationGame.TicksPerHour, 160)
        };

        private static Color blendColor;
        private static Color darknessColor;

        public static void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            inverseSoftLightning = SimulationGame.ContentManager.Load<Texture2D>(@"Lightning\InverseSoft100");
            softLightning = SimulationGame.ContentManager.Load<Texture2D>(@"Lightning\Soft100");

            renderTarget = new RenderTarget2D(
                SimulationGame.Graphics.GraphicsDevice,
                SimulationGame.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                SimulationGame.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SimulationGame.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public static void AddLightning(Vector2 position, Lightning lightning)
        {
            lightnings.Add(new LightningPosition()
            {
                Position=position,
                Lightning=lightning
            });
        }

        private static void setDarknessColor()
        {
            if (SimulationGame.Player.InteriorID != Interior.Outside)
            {
                darknessColor = new Color(0, 0, 0, 0);

                return;
            }

            int currentDayTick = TimeUtils.GetCurrentDayTick();

            int lowerBound = 0;
            int upperBound = 0;
            int from = 0;
            int to = 0;

            for (int i = 0; i < dayNightCycleColors.Count; i++)
            {
                if (currentDayTick < dayNightCycleColors[i].Item1 || i == dayNightCycleColors.Count - 1)
                {
                    if (i == dayNightCycleColors.Count - 1 && currentDayTick >= dayNightCycleColors[i].Item1)
                    {
                        from = dayNightCycleDarkness[i].Item2;
                        to = dayNightCycleDarkness[0].Item2;

                        lowerBound = dayNightCycleColors[i].Item1;
                        upperBound = 24 * SimulationGame.TicksPerHour;
                    }
                    else
                    {
                        from = dayNightCycleDarkness[i - 1].Item2;
                        to = dayNightCycleDarkness[i].Item2;

                        lowerBound = dayNightCycleColors[i - 1].Item1;
                        upperBound = dayNightCycleColors[i].Item1;
                    }

                    break;
                }
            }

            float distance = (float)(currentDayTick - lowerBound) / (float)(upperBound - lowerBound);

            darknessColor = new Color(
                0,
                0,
                0,
                from + (int)(distance * (to - from))
            );
        }

        private static void setBlendColor()
        {
            if (SimulationGame.Player.InteriorID != Interior.Outside)
            {
                blendColor = Color.White;

                return;
            }

            int currentDayTick = TimeUtils.GetCurrentDayTick();

            int lowerBound = 0;
            int upperBound = 0;
            Color from = Color.White;
            Color to = Color.White;

            for (int i = 0; i < dayNightCycleColors.Count; i++)
            {
                if (currentDayTick < dayNightCycleColors[i].Item1 || i == dayNightCycleColors.Count - 1)
                {
                    if (i == dayNightCycleColors.Count - 1 && currentDayTick >= dayNightCycleColors[i].Item1)
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

            blendColor = new Color(
                from.R + (int)(distance * (to.R - from.R)),
                from.G + (int)(distance * (to.G - from.G)),
                from.B + (int)(distance * (to.B - from.B)),
                255
            );
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (SimulationGame.IsSurroundingEffectsOff)
            {
                return;
            }
            else
            {
                setBlendColor();
                setDarknessColor();
            }

            if (SimulationGame.Player.InteriorID != Interior.Outside)
            {
                blendColor = Color.White;

                return;
            }

            var graphicsDevice = SimulationGame.Graphics.GraphicsDevice;

            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.White);

            BlendState lighningBlend = new BlendState();

            lighningBlend.AlphaBlendFunction = BlendFunction.Min;
            lighningBlend.AlphaSourceBlend = Blend.Zero;
            lighningBlend.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            lighningBlend.ColorBlendFunction = BlendFunction.Min;
            lighningBlend.ColorSourceBlend = Blend.Zero;
            lighningBlend.ColorDestinationBlend = Blend.InverseSourceColor;

            spriteBatch.Begin(SpriteSortMode.Deferred, lighningBlend);

            foreach(var lightning in lightnings)
            {
                var cameraPosition = SimulationGame.ConvertWorldPositionToUIPosition(lightning.Position.X + lightning.Lightning.RelativePosition.X, lightning.Position.Y + lightning.Lightning.RelativePosition.Y);

                spriteBatch.Draw(inverseSoftLightning, new Rectangle((int)(cameraPosition.X - lightning.Lightning.Radius), (int)(cameraPosition.Y - lightning.Lightning.Radius), lightning.Lightning.Radius * 2, lightning.Lightning.Radius * 2), Color.White);
            }

            // Player Lightning
            spriteBatch.Draw(inverseSoftLightning, new Rectangle(SimulationGame.Resolution.Width / 2 - 150, SimulationGame.Resolution.Height / 2 - 150, 300, 300), Color.White);

            spriteBatch.End();

            // Drop the render target
            graphicsDevice.SetRenderTarget(null);

            // Create blendState
            BlendState blendState = new BlendState();
            blendState.ColorSourceBlend = Blend.DestinationColor;

            // Step 1: Draw Time Overlay To World
            spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), blendColor);
            spriteBatch.End();

            // Step 2: Draw Time Darkness To World
            spriteBatch.Begin(SpriteSortMode.Immediate);
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), darknessColor);
            spriteBatch.End();

            // Step 3: Draw Lightnings
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            foreach (var lightning in lightnings)
            {
                var cameraPosition = SimulationGame.ConvertWorldPositionToUIPosition(lightning.Position.X + lightning.Lightning.RelativePosition.X, lightning.Position.Y + lightning.Lightning.RelativePosition.Y);

                spriteBatch.Draw(softLightning, new Rectangle((int)(cameraPosition.X - lightning.Lightning.Radius), (int)(cameraPosition.Y - lightning.Lightning.Radius), lightning.Lightning.Radius * 2, lightning.Lightning.Radius * 2), lightning.Lightning.Color);
            }
            
            spriteBatch.End();

            lightnings.Clear();
        }
    }
}
