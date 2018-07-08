using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public class AmbientHitableObject: HitableObject
    {
        public AmbientHitableObjectType ambientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject() { }

        public AmbientHitableObject(AmbientHitableObjectType ambientHitableObjectType, Vector2 position, Rectangle relativeBlockingRectangle):
            base(position, relativeBlockingRectangle, World.BlockingType.BLOCKING) {
            this.ambientHitableObjectType = ambientHitableObjectType;
        }
    }
}
