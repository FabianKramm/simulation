using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    class DrawableObject
    {
        protected Point position;

        public DrawableObject(Point position)
        {
            this.position = position;
        }

        public void moveTo(Point p)
        {
            position = p;
        }

        public void moveX(int distance)
        {
            position.X += distance;
        }

        public void moveY(int distance)
        {
            position.Y += distance;
        }

        public void move(Vector2 vector)
        {
            position.X = (int)(position.X + vector.X);
            position.Y = (int)(position.X + vector.X);
        }
    }
}
