using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.AI.Tasks
{
    public class FleeTask: BehaviorTask
    {
        public static readonly string ID = "FleeTask";

        private float maxDistance;
        private GameObject from;

        public FleeTask(MovingEntity subject, GameObject from, float maxDistance): base(subject)
        {
            this.from = from;
            this.maxDistance = maxDistance;
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingEntity = (MovingEntity)subject;

            if (GeometryUtils.VectorsWithinDistance(movingEntity.Position, from.Position, maxDistance) == false)
            {
                movingEntity.StopWalking();

                return BehaviourTreeStatus.Success;
            }
            else
            {
                var neighbors = new SortedList<float, Point>();
                var originalDistanceToTarget = GeometryUtils.GetDiagonalDistance(movingEntity.BlockPosition.X, movingEntity.BlockPosition.Y, from.BlockPosition.X, from.BlockPosition.Y);

                // Find blockPosition to walk to
                for (int i=-1;i<=1;i++)
                    for(int j=-1;j<=1;j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        var neighborBlockPosition = new Point(i + movingEntity.BlockPosition.X, j + movingEntity.BlockPosition.Y);
                        var distanceToTarget = GeometryUtils.GetDiagonalDistance(neighborBlockPosition.X, neighborBlockPosition.Y, from.BlockPosition.X, from.BlockPosition.Y);

                        if (distanceToTarget > originalDistanceToTarget && CollisionUtils.IsBlockPositionWalkable(neighborBlockPosition.X, neighborBlockPosition.Y, movingEntity.InteriorID))
                        {
                            neighbors.Add(distanceToTarget, neighborBlockPosition);
                        }
                    }

                if(neighbors.Count > 0)
                {
                    Point pointToWalkTo = neighbors.Values[neighbors.Count - 1];

                    movingEntity.WalkToPosition(new WorldPosition(pointToWalkTo.X * WorldGrid.BlockSize.X + 15, pointToWalkTo.Y * WorldGrid.BlockSize.Y + 15, movingEntity.InteriorID));

                    return BehaviourTreeStatus.Running;
                }

                return BehaviourTreeStatus.Failure;
            }
        }
    }
}
