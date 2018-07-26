using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Renderer;
using Simulation.Game.World;
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

        public static AmbientHitableObject createTree(WorldPosition position)
        {
            return new AmbientHitableObject(AmbientHitableObjectType.TREE01, new WorldPosition(position.X, position.Y + WorldGrid.BlockSize.Y - 1, position.InteriorID), new Util.Geometry.Rect(6, -36, 67, 36));
        }

        public static AmbientObject createSmallRocks(WorldPosition position)
        {
            return new AmbientObject((AmbientObjectType)random.Next((int)AmbientObjectType.SMALL_ROCK01, (int)AmbientObjectType.SMALL_ROCK05 + 1), new WorldPosition(position.X - 12.5f + World.WorldGrid.BlockSize.X / 2, position.Y + 10 + World.WorldGrid.BlockSize.Y / 2, position.InteriorID));
        }
    }
}
