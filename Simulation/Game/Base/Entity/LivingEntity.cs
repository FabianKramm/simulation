using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer;

namespace Simulation.Game.Base
{
    public class LivingEntity: HitableObject
    {
        public LivingEntityType livingEntityType
        {
            get; private set;
        }

        public LivingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.livingEntityType = livingEntityType;
        }
    }
}
