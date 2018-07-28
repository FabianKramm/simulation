using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Enums;
using Simulation.Game.Fractions;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.Skills;
using Simulation.Game.World;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Objects.Entities
{
    public abstract class LivingEntity: HitableObject
    {
        private static readonly TimeSpan lifeRegenInterval = TimeSpan.FromMilliseconds(500);
        public LivingEntityRendererInformation RendererInformation;

        public int LivingEntityType;
        
        public BaseAI BaseAI;
        public Skill[] Skills;

        public int AttentionBlockRadius;
        public int MaximumLife;
        public int CurrentLife;
        public float LifeRegeneration;
        public FractionType Fraction;

        private Dictionary<string, int> aggroLookup = new Dictionary<string, int>();
        private TimeSpan timeTillLifeRegen = TimeSpan.Zero;

        // Create from JSON
        protected LivingEntity() : base() { }

        protected LivingEntity(WorldPosition worldPosition): base(worldPosition) { }

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
       
        public bool IsDead()
        {
            return CurrentLife <= 0;
        }

        public void SetAttentionRadius(int attentionRadius)
        {
            AttentionBlockRadius = attentionRadius;
        }

        public void SetAI(BaseAI baseAI)
        {
            this.BaseAI = baseAI;
        }

        public void Talk(string line)
        {
            if(RendererInformation != null)
            {
                RendererInformation.SpeechLine = line;
            }
        }

        public void ModifyHealth(int modifier)
        {
            CurrentLife = Math.Max(0, Math.Min(MaximumLife, CurrentLife + modifier));

            if(CurrentLife <= 0)
            {
                // Add die effect

                DisconnectFromWorld();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if(BaseAI != null)
                BaseAI.Update(gameTime);

            if (Skills != null)
                foreach (var skill in Skills)
                    skill.Update(gameTime);

            timeTillLifeRegen += gameTime.ElapsedGameTime;

            if(timeTillLifeRegen >= lifeRegenInterval)
            {
                ModifyHealth((int)(LifeRegeneration * timeTillLifeRegen.TotalMilliseconds));
                timeTillLifeRegen = TimeSpan.Zero;
            }
        }
    }
}
