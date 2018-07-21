using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.Skills
{
    public abstract class Skill
    {
        public int Cooldown;

        protected LivingEntity owner;
        protected TimeSpan cooldownLeft = TimeSpan.Zero;

        protected Skill(LivingEntity owner)
        {
            this.owner = owner;
        }

        public Skill(LivingEntity owner, int cooldown = 0)
        {
            this.owner = owner;
            this.Cooldown = cooldown;
        }

        public virtual bool IsReady()
        {
            return cooldownLeft.Milliseconds <= 0;
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
