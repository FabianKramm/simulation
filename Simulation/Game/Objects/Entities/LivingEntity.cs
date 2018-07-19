using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.AI;
using Simulation.Game.Enums;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.Objects.Entities
{
    public abstract class LivingEntity: HitableObject
    {
        public LivingEntityRendererInformation RendererInformation;
        public BaseAI BaseAI;

        public FractionType Fraction
        {
            get; private set;
        }
        
        public LivingEntityType LivingEntityType
        {
            get; private set;
        }

        private List<LivingEntity> targets = new List<LivingEntity>();

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, FractionType fraction) : 
            base(position, relativeHitBoxBounds)
        {
            this.LivingEntityType = livingEntityType;
            this.Fraction = fraction;
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
