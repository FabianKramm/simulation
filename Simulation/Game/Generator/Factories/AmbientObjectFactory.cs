using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Renderer;
using System;

namespace Simulation.Game.Generator.Factories
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
            return new AmbientHitableObject(AmbientHitableObjectType.TREE01, new Vector2(position.X, position.Y + World.WorldGrid.BlockSize.Y - 1), new Util.Geometry.Rect(6, -36, 67, 36));
        }

        public static AmbientObject createSmallRocks(Vector2 position)
        {
            return new AmbientObject((AmbientObjectType)random.Next((int)AmbientObjectType.SMALL_ROCK01, (int)AmbientObjectType.SMALL_ROCK05 + 1), new Vector2(position.X - 12.5f + World.WorldGrid.BlockSize.X / 2, position.Y + 10 + World.WorldGrid.BlockSize.Y / 2));
        }
    }
}
