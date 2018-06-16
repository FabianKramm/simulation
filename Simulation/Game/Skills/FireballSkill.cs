using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Basics;
using Simulation.Game.Effect;
using Simulation.Game.Hud;
using Simulation.Spritesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Skills
{
    public class FireballSkill: Skill
    {
        private Vector2 relativeOriginPosition;

        public FireballSkill(LivingEntity owner, Vector2? relativeOriginPosition = null) : base(owner, 500)
        {
            this.relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.isDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Fireball");
            }

            SimulationGame.effects.Add(new Fireball(owner, targetPosition, relativeOriginPosition));
        }
    }
}
