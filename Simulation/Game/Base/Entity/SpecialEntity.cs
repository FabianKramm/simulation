using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Base.Entity
{
    public class SpecialEntity: MovingEntity
    {


        public SpecialEntity(Vector2 position, Rectangle relativeHitBoxBounds) :
            base(position, relativeHitBoxBounds)
        {

        }
    }
}
