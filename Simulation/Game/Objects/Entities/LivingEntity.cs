using Microsoft.Xna.Framework;
using Simulation.Game.Fractions;
using Simulation.Game.MetaData;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Scripts.Base;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Objects.Entities
{
    public abstract class LivingEntity: HitableObject
    {
        private static readonly TimeSpan resurrectionTimespan = TimeSpan.FromMilliseconds(10000);
        private static readonly TimeSpan lifeRegenInterval = TimeSpan.FromMilliseconds(1000);

        public LivingEntityRendererInformation RendererInformation;
        public Skill[] Skills;

        [Serialize]
        public int LivingEntityType;

        [Serialize]
        public int MaximumLife;

        [Serialize]
        public int CurrentLife;

        [Serialize]
        public float LifeRegeneration;

        [Serialize]
        public FractionType Fraction;

        [Serialize]
        public bool Invincible;

        private TimeSpan timeToResurrect;
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

        public override void Init()
        {
            relativeBlockingBounds = MetaData.LivingEntityType.lookup[((LivingEntity)this).LivingEntityType].RelativeBlockingBounds;
            relativeHitBoxBounds = MetaData.LivingEntityType.lookup[((LivingEntity)this).LivingEntityType].RelativeHitBoxBounds;

            base.Init();

            // Init skills
            var livingEntityType = GetObjectType();

            Skills = new Skill[livingEntityType.Skills == null ? 0 : livingEntityType.Skills.Length];

            if(livingEntityType.Skills != null)
                for (int i=0;i<livingEntityType.Skills.Length;i++)
                    Skills[i] = SkillType.Create(this, livingEntityType.Skills[i]);
      
            // Init 
            if (livingEntityType.CustomControllerScript != null)
                CustomController = (GameObjectController)SerializationUtils.CreateInstance(livingEntityType.CustomControllerScript);

            if (livingEntityType.CustomRendererScript != null)
                CustomRenderer = (GameObjectRenderer)SerializationUtils.CreateInstance(livingEntityType.CustomRendererScript);

            CustomController?.Init(this);
            CustomRenderer?.Init(this);

            SimulationGame.World.LivingEntities[ID] = this;
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

        public void Talk(string line)
        {
            if(RendererInformation != null)
            {
                RendererInformation.SpeechLine = line;
                RendererInformation.ShowSpeechTimeout = TimeSpan.FromMilliseconds(Math.Max(1000, line.Length * 70));
            }
        }

        public void ModifyHealth(int modifier, LivingEntity from)
        {
            if (IsDead())
                return;

            CurrentLife = Math.Max(0, Math.Min(MaximumLife, CurrentLife + modifier));

            if(CurrentLife <= 0)
            {
                if(Invincible == false)
                {
                    // Add die effect

                    DisconnectFromWorld();
                    Destroy();
                }
                else
                {
                    CurrentLife = 0;
                    timeToResurrect = resurrectionTimespan;
                }
            }

            // Change Attitude towards other person
            if(from != null && modifier < 0)
            {
                if(GetAggroTowardsEntity(from) < (int)FractionRelationType.ALLIED)
                {
                    aggroLookup[from.ID] = GetAggroTowardsEntity(from) + modifier;
                }
            }
        }

        public LivingEntityType GetObjectType()
        {
            return MetaData.LivingEntityType.lookup[LivingEntityType];
        }

        public override bool IsBlocking()
        {
            return IsDead() == false && isBlocking;
        }

        public override bool IsHitable()
        {
            return IsDead() == false && isHitable;
        }

        public override void Destroy()
        {
            SimulationGame.World.LivingEntities.Remove(ID);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead())
            {
                timeToResurrect -= gameTime.ElapsedGameTime;

                if(timeToResurrect.TotalMilliseconds <= 0)
                {
                    CurrentLife = (int)(MaximumLife * 0.25f);
                }
            }
            else
            {
                base.Update(gameTime);

                if (Skills != null)
                    foreach (var skill in Skills)
                        skill.Update(gameTime);

                timeTillLifeRegen += gameTime.ElapsedGameTime;

                if (timeTillLifeRegen >= lifeRegenInterval)
                {
                    ModifyHealth((int)(LifeRegeneration * timeTillLifeRegen.TotalMilliseconds), null);
                    timeTillLifeRegen = TimeSpan.Zero;
                }
            }

            if(RendererInformation != null)
            {
                if(RendererInformation.SpeechLine != null)
                {
                    RendererInformation.ShowSpeechTimeout -= gameTime.ElapsedGameTime;

                    if(RendererInformation.ShowSpeechTimeout.TotalMilliseconds <= 0)
                    {
                        RendererInformation.SpeechLine = null;
                    }
                }
            }
        }
    }
}
