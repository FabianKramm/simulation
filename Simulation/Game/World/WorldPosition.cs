using Microsoft.Xna.Framework;
using Simulation.Util.Geometry;

namespace Simulation.Game.World
{
    public class WorldPosition
    {
        public int X;
        public int Y;

        public string InteriorID;

        public Point BlockPosition
        {
            get => GeometryUtils.GetChunkPosition((int)X, (int)Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public bool IsOutside
        {
            get => InteriorID == Interior.Outside;
        }

        public Vector2 ToVector() => new Vector2(X, Y);

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
            X = (int)position.X;
            Y = (int)position.Y;
            InteriorID = Interior.Outside;
        }

        public WorldPosition(float x, float y)
        {
            X = (int)x;
            Y = (int)y;
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
            X = (int)position.X;
            Y = (int)position.Y;
            InteriorID = interiorID;
        }

        public WorldPosition(float x, float y, string interiorID)
        {
            X = (int)x;
            Y = (int)y;
            InteriorID = interiorID;
        }
    }
}
