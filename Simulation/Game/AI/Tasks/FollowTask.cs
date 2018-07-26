using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Util.Geometry;

namespace Simulation.Game.AI.Tasks
{
    public class FollowTask: BehaviorTask
    {
        public static readonly string ID = "FollowObjectTask";

        private float tillDistance;
        private GameObject target;

        public FollowTask(MovingEntity movingEntity, GameObject target, float realDistance): base(movingEntity)
        {
            this.target = target;
            this.tillDistance = realDistance;
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingEntity = (MovingEntity)subject;

            if(GeometryUtils.VectorsWithinDistance(movingEntity.Position, target.Position, tillDistance))
            {
                movingEntity.StopWalking();
                return BehaviourTreeStatus.Success;
            }
            else
            {
                // Only rewalk if new position is greater than 3 blocks
                if (movingEntity.InteriorID == target.InteriorID && movingEntity.DestBlockPosition != null && GeometryUtils.VectorsWithinDistance(movingEntity.DestBlockPosition, target.Position.ToBlockPosition(), 5))
                    return BehaviourTreeStatus.Running;

                movingEntity.WalkToPosition(target.Position);
                return BehaviourTreeStatus.Running;
            }
        }
    }
}
