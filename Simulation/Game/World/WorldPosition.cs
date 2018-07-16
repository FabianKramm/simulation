using Microsoft.Xna.Framework;

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
    }
}
