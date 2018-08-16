using Microsoft.Xna.Framework;
using Simulation.Game.Fractions;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Renderer;
using Simulation.Game.World;
using System;

namespace Simulation.Game.Generator.Factories
{
    class AmbientObjectFactory
    {
        private static Random random = new Random();

        public static AmbientHitableObject createTree(WorldPosition position)
        {
            return AmbientHitableObjectType.Create(new WorldPosition(position.X, position.Y + WorldGrid.BlockSize.Y - 1, position.InteriorID), AmbientHitableObjectType.lookup[0]);
        }

        public static AmbientObject createSmallRocks(WorldPosition position)
        {
            return AmbientObjectType.Create(new WorldPosition(position.X - 12.5f + WorldGrid.BlockSize.X / 2, position.Y + 10 + WorldGrid.BlockSize.Y / 2, position.InteriorID), AmbientObjectType.lookup[random.Next(0, 2)]);
        }
    }
}
