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
            float minValue = SimulationGame.visibleArea.Left + SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Top;

            return getLayerDepthFromPosition(minValue - (reservedDepthLayers - (int)layer), 0);
        }

        public static float getLayerDepthFromReservedLayer(int zIndex)
        {
            float minValue = SimulationGame.visibleArea.Left + SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Top;

            return getLayerDepthFromPosition(minValue - (reservedDepthLayers - zIndex), 0);
        }

        public static float getLayerDepthFromPosition(float X, float Y)
        {
            return Normalize(Y * SimulationGame.visibleArea.Width + X + reservedDepthLayers, 
                SimulationGame.visibleArea.Left + SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Top,
                SimulationGame.visibleArea.Width * SimulationGame.visibleArea.Bottom + reservedDepthLayers);
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
