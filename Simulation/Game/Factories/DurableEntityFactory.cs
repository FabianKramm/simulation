using Microsoft.Xna.Framework;
using Simulation.Game.Base.Entity;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Factories
{
    public class DurableEntityFactory
    {
        public static DurableEntity CreateGeralt()
        {
            return new DurableEntity(LivingEntityType.GERALT, new Vector2(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3), new Rectangle(-8, -20, 16, 20), 1);
        }
    }
}
