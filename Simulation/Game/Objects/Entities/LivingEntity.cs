using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Enums;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.Skills;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.Objects.Entities
{
    public abstract class LivingEntity: HitableObject
    {
        public LivingEntityRendererInformation RendererInformation;
        public BaseAI BaseAI;
        public Skill[] Skills;

        public int MaximumLife
        {
            get; protected set;
        }

        public int CurrentLife
        {
            get; protected set;
        }

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

        public void UseSkill(Skill skill, Vector2 target)
        {
            skill.Use(target);
        }

        public void SetAI(BaseAI baseAI)
        {
            this.BaseAI = baseAI;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(BaseAI != null)
                BaseAI.Update(gameTime);

            if (Skills != null)
                foreach (var skill in Skills)
                    skill.Update(gameTime);
        }
    }
}
