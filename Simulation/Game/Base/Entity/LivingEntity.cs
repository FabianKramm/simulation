using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Renderer.Entities;

namespace Simulation.Game.Base.Entity
{
    public abstract class LivingEntity: HitableObject
    {
        public LivingEntityRendererInformation RendererInformation;

        public LivingEntityType LivingEntityType
        {
            get; private set;
        }

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.LivingEntityType = livingEntityType;
        }

        public abstract void Update(GameTime gameTime);
    }
}
