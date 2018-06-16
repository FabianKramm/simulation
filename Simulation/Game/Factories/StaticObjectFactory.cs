using Microsoft.Xna.Framework;
using Simulation.Game.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Factories
{
    class StaticObjectFactory
    {
        public static StaticObject createTree(Vector2 position)
        {
            return new StaticObject(@"Environment\Tree01", new Rectangle(0, 0, 79, 91), position, new Point(6, 55), new Point(67, 36), World.CollisionType.SOLID_OBJECT);
        }
    }
}
