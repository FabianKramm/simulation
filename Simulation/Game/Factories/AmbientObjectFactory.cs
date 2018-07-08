using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Renderer;
using System;

namespace Simulation.Game.Factories
{
    enum StaticObjects
    {
        Tree01 = 0,
    }

    class AmbientObjectFactory
    {
        private static Random random = new Random();

        public static AmbientHitableObject createTree(Vector2 position)
        {
            return new AmbientHitableObject(AmbientHitableObjectType.TREE01, new Vector2(position.X, position.Y + World.World.BlockSize.Y), new Rectangle(6, -36, 67, 36));
        }

        public static AmbientObject createSmallRocks(Vector2 position)
        {
            return new AmbientObject((AmbientObjectType)random.Next((int)AmbientObjectType.SMALL_ROCK01, (int)AmbientObjectType.SMALL_ROCK05 + 1), new Vector2(position.X - 12.5f + World.World.BlockSize.X / 2, position.Y + 10 + World.World.BlockSize.Y / 2));
        }
    }
}
