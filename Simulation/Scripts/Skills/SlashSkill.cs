using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;
using Simulation.Game.World;
using Simulation.Game.Serialization;
using Newtonsoft.Json.Linq;
using Simulation.Game.Skills;
using Simulation;

namespace Scripts.Skills
{
    public class SlashSkill : Skill
    {
        public static readonly int Range = WorldGrid.BlockSize.X * 2;
        public float DamagePerHit
        {
            get; private set;
        }

        private Vector2 relativeOriginPosition;
        private bool flipped;

        public override void Init(LivingEntity owner, JObject parameters)
        {
            base.Init(owner, parameters);

            this.relativeOriginPosition = SerializationUtils.GetFromObject<Vector2>(parameters, "relativeOriginPosition", Vector2.Zero);
            this.DamagePerHit = SerializationUtils.GetFromObject<int>(parameters, "damagePerHit", 10);
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
