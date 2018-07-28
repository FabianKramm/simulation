using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        public int AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject(): base() { }

        public AmbientHitableObject(WorldPosition position): base(position) {}
    }
}
