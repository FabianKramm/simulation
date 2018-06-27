using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer;

namespace Simulation.Game.Base
{
    public class MovingEntity: LivingEntity
    {
        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) :
            base(livingEntityType, position, relativeHitBoxBounds)
        {

        }
    }
}
