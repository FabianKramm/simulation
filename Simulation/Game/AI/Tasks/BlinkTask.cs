using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Scripts.Skills;

namespace Simulation.Game.AI.Tasks
{
    public class BlinkTask: BehaviorTask
    {
        public static readonly string ID = "BlinkTask";
        private WorldPosition target;

        public BlinkTask(LivingEntity livingEntity, WorldPosition target) : base(livingEntity)
        {
            var direction = Vector2.Subtract(target.ToVector(), livingEntity.Position.ToVector());
            direction.Normalize();

            this.target = new WorldPosition(target.X - direction.X * WorldGrid.BlockSize.X, target.Y - direction.Y * WorldGrid.BlockSize.Y, target.InteriorID);
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            BlinkSkill blinkSkill = AIUtils.GetBlinkSkill(subject);

            if (blinkSkill == null || blinkSkill.IsReady() == false)
                return BehaviourTreeStatus.Failure;

            blinkSkill.Use(target.ToVector());

            return BehaviourTreeStatus.Success;
        }
    }
}
