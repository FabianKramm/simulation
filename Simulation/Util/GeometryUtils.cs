using Microsoft.Xna.Framework;
using System;

namespace Simulation.Util
{
    public enum ReservedDepthLayers
    {
        Block = 0,
        BlockDecoration = 1
    }

    public class GeometryUtils
    {
        public static int reservedDepthLayers = 100;

        public static float getLayerDepthFromReservedLayer(ReservedDepthLayers layer)
        {
            return Normalize((int)layer,
                            0,
                            SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Height + reservedDepthLayers);
        }

        public static float getLayerDepthFromReservedLayer(int zIndex)
        {
            return Normalize(zIndex,
                            0,
                            SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Height + reservedDepthLayers);
        }

        public static float getLayerDepthFromPosition(float X, float Y)
        {
            // float min = SimulationGame.visibleArea.Left + SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Top;
            // float max = SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Bottom + reservedDepthLayers;
            float value = (Y - SimulationGame.visibleArea.Top) * SimulationGame.visibleArea.Width + (X - SimulationGame.visibleArea.Left) + reservedDepthLayers;

            return Normalize(value,
                            0,
                            SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Height + reservedDepthLayers);
        }

        public static float Normalize(float value, float min, float max)
        {
            return Math.Max(0.0f, Math.Min(1.0f, (value - min) / (max - min)));
        }

        public static Vector2 Rotate(float angle, ref Vector2 pivot, ref Vector2 point)
        {
            Vector2 rotatedPoint = point;

            rotatedPoint.X -= pivot.X;
            rotatedPoint.Y -= pivot.Y;

            // Rotate about the origin.
            Matrix mat = Matrix.CreateRotationZ(angle);
            rotatedPoint = Vector2.Transform(rotatedPoint, mat);

            rotatedPoint.X += pivot.X;
            rotatedPoint.Y += pivot.Y;

            return rotatedPoint;
        }
    }
}
