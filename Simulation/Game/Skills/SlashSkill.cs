using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;

namespace Simulation.Game.Skills
{
    public class SlashSkill : Skill
    {
        private Vector2 relativeOriginPosition;
        private bool flipped;

        // Serialization
        protected SlashSkill(LivingEntity owner): base(owner) { }

        public SlashSkill(LivingEntity owner, Vector2? relativeOriginPosition = null): 
            base(owner, 200)
        {
            this.relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.IsDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Slash");
            }

            flipped = !flipped;

            SimulationGame.World.AddEffectToWorld(new Slash((MovingEntity)owner, targetPosition, flipped, relativeOriginPosition));
        }
    }
}
