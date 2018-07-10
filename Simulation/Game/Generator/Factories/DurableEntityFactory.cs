﻿using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Generator.Factories
{
    public class DurableEntityFactory
    {
        public static MovingEntity CreateGeralt()
        {
            var geralt = new MovingEntity(LivingEntityType.GERALT, new Vector2(WorldGrid.BlockSize.X * 3, WorldGrid.BlockSize.Y * 3), new Rect(-8, -20, 16, 20));

            geralt.BaseAI = new WanderAI(geralt, 5);

            return geralt;
        }
    }
}
