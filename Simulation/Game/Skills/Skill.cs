using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using System;

namespace Simulation.Game.Skills
{
    public abstract class Skill
    {
        public int Cooldown
        {
            get; protected set;
        }

        protected LivingEntity owner;
        protected TimeSpan cooldownLeft = TimeSpan.Zero;

        public virtual void Init(LivingEntity owner, JObject parameters)
        {
            this.owner = owner;
            this.Cooldown = SerializationUtils.GetFromObject<int>(parameters, "cooldown", 100);
        }

        public virtual bool IsReady()
        {
            return cooldownLeft.TotalMilliseconds <= 0;
        }

        public virtual void Use(Vector2 targetPosition)
        {
            if(IsReady())
            {
                cooldownLeft = TimeSpan.FromMilliseconds(Cooldown);
                trigger(targetPosition);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            cooldownLeft -= gameTime.ElapsedGameTime;
        }

        protected abstract void trigger(Vector2 targetPosition);
    }
}
