using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Effects;
using Simulation.Game.Hud;
using Simulation.Game.Serialization;
using Newtonsoft.Json.Linq;
using Simulation;
using Scripts.Base;

namespace Scripts.Skills
{
    public class FireballSkill: Skill
    {
        private Vector2 relativeOriginPosition;
        public float DamagePerHit
        {
            get; private set;
        }

        // Serialization
        public override void Init(LivingEntity owner, JObject parameters)
        {
            base.Init(owner, parameters);

            this.Cooldown = SerializationUtils.GetFromObject(parameters, "cooldown", 700);
            this.relativeOriginPosition = SerializationUtils.GetFromObject(parameters, "relativeOriginPosition", Vector2.Zero);
            this.DamagePerHit = SerializationUtils.GetFromObject(parameters, "damagePerHit", 10);
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
