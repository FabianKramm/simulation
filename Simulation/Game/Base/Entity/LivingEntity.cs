using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Renderer.Entities;

namespace Simulation.Game.Base
{
    public abstract class LivingEntity: HitableObject
    {
        [JsonIgnore]
        public LivingEntityRendererInformation rendererInformation;

        public LivingEntityType livingEntityType
        {
            get; private set;
        }

        public LivingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) : 
            base(position, relativeHitBoxBounds)
        {
            this.livingEntityType = livingEntityType;
        }

        public abstract void Update(GameTime gameTime);
    }
}
