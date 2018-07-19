using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.AI;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects.Entities
{
    public enum LivingEntityType
    {
        NO_ENTITY = 0,
        PLAYER,
        GERALT
    }

    public abstract class LivingEntity: HitableObject
    {
        public LivingEntityRendererInformation RendererInformation;

        public BaseAI BaseAI;

        public LivingEntityType LivingEntityType
        {
            get; private set;
        }

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.LivingEntityType = livingEntityType;
        }

        public void SetAI(BaseAI baseAI)
        {
            this.BaseAI = baseAI;
        }



        public virtual void Update(GameTime gameTime)
        {
            if(BaseAI != null)
            {
                BaseAI.Update(gameTime);
            }
        }
    }
}
