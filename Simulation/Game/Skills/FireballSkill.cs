using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;

namespace Simulation.Game.Skills
{
    public class FireballSkill: Skill
    {
        private Vector2 relativeOriginPosition;

        // Serialization
        protected FireballSkill(LivingEntity owner) : base(owner) { }

        public FireballSkill(LivingEntity owner, Vector2? relativeOriginPosition = null) : base(owner, 700)
        {
            this.relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.IsDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Fireball");
            }

            SimulationGame.World.AddEffectToWorld(new Fireball(owner, targetPosition, relativeOriginPosition));
        }
    }
}
