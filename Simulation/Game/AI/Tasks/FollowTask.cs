using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.AI.Tasks
{
    public class FollowTask: BehaviorTask
    {
        public static readonly string ID = "FollowObjectTask";

        private float tillDistance;
        private string targetID;
        private float teleportDistance;

        public FollowTask(MovingEntity movingEntity, string targetID, float realDistance, float teleportDistance = 0): base(movingEntity)
        {
            this.targetID = targetID;
            this.tillDistance = realDistance;
            this.teleportDistance = teleportDistance;
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingEntity = (MovingEntity)subject;
            var target = SimulationGame.World.LivingEntities.ContainsKey(targetID) ? SimulationGame.World.LivingEntities[targetID] : null;

            if(target == null)
            {
                movingEntity.StopWalking();
                return BehaviourTreeStatus.Failure;
            }

            if(GeometryUtils.VectorsWithinDistance(movingEntity.Position, target.Position, tillDistance))
            {
                movingEntity.StopWalking();
                return BehaviourTreeStatus.Success;
            }
            else
            {
                if(teleportDistance > 0 && movingEntity.InteriorID == target.InteriorID && GeometryUtils.GetEuclideanDistance(movingEntity.Position, target.Position) > teleportDistance)
                {
                    var teleportBlockDistance = teleportDistance / WorldGrid.BlockSize.X;
                    var teleportPosition = AIUtils.GetWalkablePositionCloseTo(movingEntity.Position, target.Position, (int)((teleportBlockDistance + 16) - teleportBlockDistance) * WorldGrid.BlockSize.X);

                    if(teleportPosition != null)
                    {
                        movingEntity.UpdatePosition(teleportPosition);

                        return BehaviourTreeStatus.Running;
                    }
                }

                // Only rewalk if new position is greater than 3 blocks
                if (movingEntity.InteriorID == target.InteriorID && movingEntity.DestBlockPosition != null && GeometryUtils.VectorsWithinDistance(movingEntity.DestBlockPosition, target.Position.ToBlockPosition(), 5))
                    return BehaviourTreeStatus.Running;

                movingEntity.WalkToPosition(target.Position);
                return BehaviourTreeStatus.Running;
            }
        }
    }
}
