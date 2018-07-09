using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;

namespace Simulation.Game.Base
{
    public class AmbientHitableObject: HitableObject
    {
        public AmbientHitableObjectType AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject() { }

        public AmbientHitableObject(AmbientHitableObjectType ambientHitableObjectType, Vector2 position, Rectangle relativeBlockingRectangle):
            base(position, relativeBlockingRectangle, World.BlockingType.BLOCKING) {
            this.AmbientHitableObjectType = ambientHitableObjectType;
        }
    }
}
