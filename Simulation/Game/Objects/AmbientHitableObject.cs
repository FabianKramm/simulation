using Simulation.Game.World;
using Simulation.Game.Serialization;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        [Serialize]
        public int AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject(): base() { }

        public AmbientHitableObject(WorldPosition position): base(position) {}
    }
}
