using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.Skills
{
    public abstract class Skill
    {
        protected int cooldown;
        protected LivingEntity owner;
        protected TimeSpan cooldownLeft = TimeSpan.Zero;

        public Skill(LivingEntity owner, int cooldown = 0)
        {
            this.owner = owner;
            this.cooldown = cooldown;
        }

        public virtual bool isReady()
        {
            return cooldownLeft.Milliseconds <= 0;
        }

        public virtual void use(Vector2 targetPosition)
        {
            if(isReady())
            {
                cooldownLeft = TimeSpan.FromMilliseconds(cooldown);
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
