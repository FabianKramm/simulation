using Simulation.Game.World;
using Simulation.Game.Serialization;
using Simulation.Game.MetaData;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        [Serialize]
        public int AmbientHitableObjectType;

        // Create from JSON
        protected AmbientHitableObject(): base() { }

        public AmbientHitableObject(WorldPosition position): base(position) {}

        public override void Init()
        {
            relativeBlockingBounds = MetaData.AmbientHitableObjectType.lookup[((AmbientHitableObject)this).AmbientHitableObjectType].RelativeBlockingRectangle;
            relativeHitBoxBounds = MetaData.AmbientHitableObjectType.lookup[((AmbientHitableObject)this).AmbientHitableObjectType].RelativeHitboxRectangle;

            base.Init();
        }

        public AmbientHitableObjectType GetObjectType()
        {
            return MetaData.AmbientHitableObjectType.lookup[AmbientHitableObjectType];
        }
    }
}
