using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using Simulation.Game.Enums;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        public AmbientHitableObjectType AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject() { }

        public AmbientHitableObject(AmbientHitableObjectType ambientHitableObjectType, WorldPosition position, Rect relativeBlockingRectangle):
            base(position, relativeBlockingRectangle, BlockingType.BLOCKING) {
            this.AmbientHitableObjectType = ambientHitableObjectType;
        }
    }
}
