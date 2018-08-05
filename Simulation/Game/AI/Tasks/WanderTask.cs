using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Generator;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System;
using System.Threading.Tasks;

namespace Simulation.Game.AI.Tasks
{
    public class WanderTask: BehaviorTask
    {
        private Task<Point> findNextWalkablePoint;

        private Circle wanderCircle;
        private string interiorID;
        
        public WanderTask(MovingEntity subject, WorldPosition blockStartPosition, int blockRadius) : base(subject)
        {
            this.wanderCircle = new Circle((int)blockStartPosition.X, (int)blockStartPosition.Y, blockRadius);
            this.interiorID = blockStartPosition.InteriorID;
        }

        public override void Start()
        {
            base.Start();

            findNextWalkablePoint = getWanderPoint();
        }

        private Task<Point> getWanderPoint()
        {
            Circle findCircle = wanderCircle; // Clone circle to avoid concurrency problems
            Random random = new Random();

            return Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Point randomPoint = GeneratorUtils.GetRandomPointInCircle(random, findCircle);

                    var realPosition = new WorldPosition(randomPoint.X * WorldGrid.BlockSize.X, randomPoint.Y * WorldGrid.BlockSize.Y, interiorID);
                    var isBlockWalkable = CollisionUtils.IsRealPositionWalkable(realPosition);
                    var worldLink = SimulationGame.World.GetWorldLinkFromRealPosition(realPosition);

                    if (isBlockWalkable && worldLink == null)
                        return randomPoint;
                }

                return Point.Zero;
            });
        }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            var movingSubject = (MovingEntity)subject;

            if (!movingSubject.IsWalking)
            {
                if (findNextWalkablePoint == null)
                {
                    return BehaviourTreeStatus.Success;
                }
                else if (findNextWalkablePoint != null && findNextWalkablePoint.IsCompleted)
                {
                    Point destBlock = findNextWalkablePoint.Result;

                    if (wanderCircle.Contains(destBlock) && interiorID == movingSubject.InteriorID)
                    {
                        movingSubject.WalkToPosition(new WorldPosition(destBlock.X * WorldGrid.BlockSize.X + 16, destBlock.Y * WorldGrid.BlockSize.Y + 16, interiorID));
                    }
                    else
                    {
                        return BehaviourTreeStatus.Success;
                    }

                    findNextWalkablePoint = null;
                }
            }

            return BehaviourTreeStatus.Running;
        }
    }
}
