using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Enums
{
    public enum HitBoxType
    {
        NO_HITBOX = 0,
        HITABLE_BLOCK,
        STATIC_OBJECT,
        MOVING_OBJECT,
        LIVING_ENTITY
    }
}
