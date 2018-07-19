using Microsoft.Xna.Framework;
using Simulation.Game.World;

namespace Simulation.Util.Geometry
{
    public struct Rect
    {
        public static readonly Rect Empty = new Rect(0, 0, 0, 0);

        public static Rect Union(Rect rect1, Rect rect2)
        {
            // Doesn't have to be efficient so we just use the XNA one
            return new Rect(Rectangle.Union(rect1.ToXnaRectangle(), rect2.ToXnaRectangle()));
        }

        public int X;
        public int Y;

        public int Width;
        public int Height;

        public int Left
        {
            get
            {
                return X;
            }
        }

        public int Top
        {
            get
            {
                return Y;
            }
        }

        public int Right
        {
            get
            {
                return X + Width - 1;
            }
        }

        public int Bottom
        {
            get
            {
                return Y + Height - 1;
            }
        }

        public Rect(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;

            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;
        }

        public bool Intersects(Rect rect)
        {
            return (X <= rect.Right) && (Right >= rect.X) && (Y <= rect.Bottom) && (Bottom >= rect.Y);
        }

        public bool Contains(Vector2 point)
        {
            return (point.X >= X) && (point.X <= Right) && (point.Y >= Y) && (point.Y <= Bottom);
        }

        public bool Contains(WorldPosition point)
        {
            return (point.X >= X) && (point.X <= Right) && (point.Y >= Y) && (point.Y <= Bottom);
        }

        public bool Contains(Point point)
        {
            return (point.X >= X) && (point.X <= Right) && (point.Y >= Y) && (point.Y <= Bottom);
        }

        public bool Contains(int x, int y)
        {
            return (x >= X) && (x <= Right) && (y >= Y) && (y <= Bottom);
        }

        public Rectangle ToXnaRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
}
