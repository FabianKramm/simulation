using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;
using Simulation.Game.Serialization;

namespace Simulation.Game.Skills
{
    public class FireballSkill: Skill
    {
        [Serialize]
        private Vector2 relativeOriginPosition;

        [Serialize]
        public float DamagePerHit
        {
            get; private set;
        }

        // Serialization
        protected FireballSkill(LivingEntity owner) : base(owner) { }

        public FireballSkill(LivingEntity owner, float damagePerHit, Vector2? relativeOriginPosition = null) : base(owner, 700)
        {
            this.relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            this.DamagePerHit = damagePerHit;
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.IsDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Fireball");
            }

            SimulationGame.World.AddEffectToWorld(new Fireball(owner, targetPosition, DamagePerHit, relativeOriginPosition));
        }
    }
}
