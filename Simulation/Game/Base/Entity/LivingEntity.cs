using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Renderer.Entities;

namespace Simulation.Game.Base.Entity
{
    public abstract class LivingEntity: HitableObject
    {
        public LivingEntityRendererInformation rendererInformation;

        public LivingEntityType livingEntityType
        {
            get; private set;
        }

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.livingEntityType = livingEntityType;
        }

        public abstract void Update(GameTime gameTime);
    }
}
