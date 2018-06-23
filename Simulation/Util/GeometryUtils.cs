using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int getIndexFromPoint(int realX, int realY, int chunkWidth, int chunkHeight)
        {
            return (realX < 0 ? -realX - 1 : realX) % chunkWidth + chunkWidth * ((realY < 0 ? -realY - 1 : realY) % chunkHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point getChunkPosition(int realX, int realY, int chunkWidth, int chunkHeight)
        {
            return new Point((int)Math.Floor((double)realX / chunkWidth), (int)Math.Floor((double)realY / chunkHeight));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point getPositionWithinChunk(int realX, int realY, int chunkWidth, int chunkHeight)
        {
            return new Point(realX - (int)Math.Floor((double)realX / chunkWidth) * chunkWidth, realY - (int)Math.Floor((double)realY / chunkHeight) * chunkHeight);
        }
    }
}
