using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;
using Simulation.Game.World;
using Simulation.Game.Serialization;

namespace Simulation.Game.Skills
{
    public class SlashSkill : Skill
    {
        public static readonly int Range = WorldGrid.BlockSize.X * 2;

        [Serialize]
        public float DamagePerHit
        {
            get; private set;
        }

        [Serialize]
        private Vector2 relativeOriginPosition;
        private bool flipped;

        // Serialization
        protected SlashSkill(LivingEntity owner): base(owner) { }

        public SlashSkill(LivingEntity owner, float damagePerHit, Vector2? relativeOriginPosition = null): 
            base(owner, 200)
        {
            this.relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            this.DamagePerHit = damagePerHit;
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.IsDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Slash");
            }

            flipped = !flipped;

            SimulationGame.World.AddEffectToWorld(new Slash((MovingEntity)owner, targetPosition, DamagePerHit, flipped, relativeOriginPosition));
        }
    }
}
