using Simulation.Game.World;
using Simulation.Game.Serialization;
using Simulation.Game.MetaData;
using Simulation.Scripts.Base;
using Simulation.Spritesheet;

namespace Simulation.Game.Objects
{
    public class AmbientHitableObject: HitableObject
    {
        public Animation ObjectAnimation;

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

            var ambientHitableObjectType = GetObjectType();

            if (ambientHitableObjectType.CustomControllerScript != null)
                CustomController = (GameObjectController)SerializationUtils.CreateInstance(ambientHitableObjectType.CustomControllerScript);

            if (ambientHitableObjectType.CustomRendererScript != null)
                CustomRenderer = (GameObjectRenderer)SerializationUtils.CreateInstance(ambientHitableObjectType.CustomRendererScript);

            CustomController?.Init(this);
            CustomRenderer?.Init(this);
        }

        public AmbientHitableObjectType GetObjectType()
        {
            return MetaData.AmbientHitableObjectType.lookup[AmbientHitableObjectType];
        }
    }
}
