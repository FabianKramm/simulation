using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.Effects;
using Simulation.Game.Hud;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Scripts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Scripts.Skills
{
    public class BlinkSkill: Skill
    {
        public static readonly int Range = WorldGrid.BlockSize.X * 2;

        private int blockDistance;

        public override void Init(LivingEntity owner, JObject parameters)
        {
            base.Init(owner, parameters);
            
            this.blockDistance = SerializationUtils.GetFromObject(parameters, "BlockDistance", 8);
            this.Cooldown = SerializationUtils.GetFromObject(parameters, "cooldown", 800);
        }

        protected override void trigger(Vector2 targetPosition)
        {
            if (SimulationGame.IsDebug)
            {
                GameConsole.WriteLine("USE_SKILL", "Blink");
            }

            var distance = Vector2.Subtract(targetPosition, owner.Position.ToVector());

            if (distance.Length() > blockDistance * WorldGrid.BlockSize.X)
            {
                distance.Normalize();
                distance = new Vector2(distance.X * blockDistance * WorldGrid.BlockSize.X, distance.Y * blockDistance * WorldGrid.BlockSize.Y);
            }

            var movingEntity = (MovingEntity)owner;
            var newPosition = new WorldPosition(movingEntity.Position.X + distance.X, movingEntity.Position.Y + distance.Y, movingEntity.InteriorID);

            SimulationGame.World.AddEffectToWorld(new Blink(owner, newPosition));
        }
    }
}
