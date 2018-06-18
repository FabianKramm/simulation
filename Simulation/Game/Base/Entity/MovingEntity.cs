using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Base
{
    public class MovingEntity: LivingEntity
    {
        public MovingEntity(Vector2 position, Rectangle relativeHitBoxBounds) :
            base(position, relativeHitBoxBounds)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
