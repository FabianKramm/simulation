using Microsoft.Xna.Framework;
using Simulation.Util.Geometry;

namespace Simulation.Game.World
{
    public class WorldPosition
    {
        public float X;
        public float Y;

        public string InteriorID;

        public bool IsOutside
        {
            get => InteriorID == Interior.Outside;
        }

        public WorldPosition Clone()
        {
            return new WorldPosition(X, Y, InteriorID);
        }

        public WorldPosition()
        {
            X = 0;
            Y = 0;
            InteriorID = Interior.Outside;
        }

        public WorldPosition(Point position)
        {
            X = position.X;
            Y = position.Y;
            InteriorID = Interior.Outside;
        }

        public WorldPosition(Vector2 position)
        {
            X = position.X;
            Y = position.Y;
            InteriorID = Interior.Outside;
        }

        public WorldPosition(float x, float y)
        {
            X = x;
            Y = y;
            InteriorID = Interior.Outside;
        }

        public WorldPosition(Point position, string interiorID)
        {
            X = position.X;
            Y = position.Y;
            InteriorID = interiorID;
        }

        public WorldPosition(Vector2 position, string interiorID)
        {
            X = position.X;
            Y = position.Y;
            InteriorID = interiorID;
        }

        public WorldPosition(float x, float y, string interiorID)
        {
            X = x;
            Y = y;
            InteriorID = interiorID;
        }

        public Point ToPoint() => new Point((int)X, (int)Y);

        public Vector2 ToVector() => new Vector2(X, Y);

        public Point ToRealPositionPoint()
        {
            return new Point((int)X * WorldGrid.BlockSize.X, (int)Y * WorldGrid.BlockSize.Y);
        }

        public Vector2 ToRealPositionVector()
        {
            Point realPosition = ToRealPositionPoint();

            return new Vector2(realPosition.X, realPosition.Y);
        }

        public WorldPosition ToRealPosition()
        {
            return new WorldPosition(X * WorldGrid.BlockSize.X, Y * WorldGrid.BlockSize.Y, InteriorID);
        }

        public Point ToBlockPositionPoint()
        {
            return GeometryUtils.GetChunkPosition((int)X, (int)Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public Vector2 ToBlockPositionVector()
        {
            Point blockPosition = ToBlockPositionPoint();

            return new Vector2(blockPosition.X, blockPosition.Y);
        }

        public WorldPosition ToBlockPosition()
        {
            Point blockPosition = GeometryUtils.GetChunkPosition((int)X, (int)Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            return new WorldPosition(blockPosition.X, blockPosition.Y, InteriorID);
        }
    }
}
