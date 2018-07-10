using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Renderer.Entities;
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

        public LivingEntityType LivingEntityType
        {
            get; private set;
        }

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, Vector2 position, Rect relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.LivingEntityType = livingEntityType;
        }

        public abstract void Update(GameTime gameTime);
    }
}
