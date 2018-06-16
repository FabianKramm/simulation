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
        private static Random random = new Random();
        private static string[] rocksTextures = new string[]
        {
            @"Environment\Rock01",
            @"Environment\Rock02",
            @"Environment\Rock03",
            @"Environment\Rock04",
            @"Environment\Rock05",
        };

        public static StaticObject createTree(Vector2 position)
        {
            int textureWidth = 79;
            int textureHeight = 91;

            return new StaticObject(@"Environment\Tree01", new Rectangle(0, 0, textureWidth, textureHeight), new Vector2(position.X, position.Y + World.World.BlockSize.Y - textureHeight), new Point(6, 55), new Point(67, 36), World.CollisionType.SOLID_OBJECT);
        }

        public static StaticSoftObject createSmallRocks(Vector2 position)
        {
            return new StaticSoftObject(rocksTextures[random.Next(0, rocksTextures.Length - 1)], new Rectangle(0, 0, 25, 20), new Vector2(position.X - 12.5f + World.World.BlockSize.X / 2, position.Y - 10 + World.World.BlockSize.Y / 2));
        }
    }
}
