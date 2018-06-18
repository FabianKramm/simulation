using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.Game.Base
{
    public class LivingEntity: HitableObject
    {
        public LivingEntity(Vector2 position, Rectangle relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds )
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
