using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        public AmbientHitableObjectType AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject() { }

        public AmbientHitableObject(AmbientHitableObjectType ambientHitableObjectType, Vector2 position, Rect relativeBlockingRectangle):
            base(position, relativeBlockingRectangle, World.BlockingType.BLOCKING) {
            this.AmbientHitableObjectType = ambientHitableObjectType;
        }
    }
}
