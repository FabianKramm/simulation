using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // GameConsole.WriteLine("", "" + getLayerDepthFromYPosition(SimulationGame.visibleArea.Top - reservedDepthLayers + 1));

            return getLayerDepthFromYPosition(SimulationGame.visibleArea.Top - (reservedDepthLayers - (int)layer));
        }

        public static float getLayerDepthFromReservedLayer(int zIndex)
        {
            return getLayerDepthFromYPosition(SimulationGame.visibleArea.Top - (reservedDepthLayers - zIndex));
        }

        public static float getLayerDepthFromYPosition(float Y)
        {
            return Normalize(Y + reservedDepthLayers, SimulationGame.visibleArea.Top, SimulationGame.visibleArea.Top + SimulationGame.resolution.Height + reservedDepthLayers);
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
