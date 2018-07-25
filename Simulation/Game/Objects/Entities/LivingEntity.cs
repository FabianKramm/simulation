using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Enums;
using Simulation.Game.Fractions;
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

        public int AttentionRadius
        {
            get; protected set;
        } = 10;

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

        private Dictionary<string, int> aggroLookup = new Dictionary<string, int>();

        // Create from JSON
        protected LivingEntity() { }

        public LivingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, FractionType fraction) : 
            base(position, relativeHitBoxBounds)
        {
            this.LivingEntityType = livingEntityType;
            this.Fraction = fraction;
        }

        public void ChangeAggroTowardsEntity(LivingEntity otherEntity, int modifier)
        {
            if (aggroLookup.ContainsKey(otherEntity.ID) == false)
            {
                aggroLookup[otherEntity.ID] = FractionRelations.GetAggro(this, otherEntity);
            }

            aggroLookup[otherEntity.ID] += modifier;
        }

        public int GetAggroTowardsEntity(LivingEntity otherEntity)
        {
            if(aggroLookup.ContainsKey(otherEntity.ID) == false)
            {
                return FractionRelations.GetAggro(this, otherEntity);
            }

            return aggroLookup[otherEntity.ID];
        }

        public void UseSkill(Skill skill, Vector2 target)
        {
            skill.Use(target);
        }

        public void SetAttentionRadius(int attentionRadius)
        {
            AttentionRadius = attentionRadius;
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
