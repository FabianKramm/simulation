using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Util.Geometry;

namespace Simulation.Game.AI.Tasks
{
    public class FollowObjectTask: BehaviorTask
    {
        public static readonly string ID = "FollowObjectTask";

        private float tillDistance;
        private GameObject target;

        public FollowObjectTask(MovingEntity movingEntity, GameObject target, float realDistance): base(movingEntity)
        {
            this.target = target;
            this.tillDistance = realDistance;
        }

        public override void Update(GameTime gameTime)
        {
            var movingEntity = (MovingEntity)subject;

            if(GeometryUtils.VectorsWithinDistance(movingEntity.Position, target.Position, tillDistance))
            {
                movingEntity.StopWalking();
                setSuccessful();
            }
            else
            {
                // Only rewalk if new position is greater than 3 blocks
                if (movingEntity.DestBlockPosition != null && GeometryUtils.VectorsWithinDistance(movingEntity.DestBlockPosition, target.Position.ToBlockPosition(), 5))
                    return;

                movingEntity.WalkToPosition(target.Position);
            }
        }
    }
}
