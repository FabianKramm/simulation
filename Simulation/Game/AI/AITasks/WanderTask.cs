using Microsoft.Xna.Framework;
using Simulation.Game.Generator;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System;
using System.Threading.Tasks;

namespace Simulation.Game.AI.AITasks
{
    public class WanderTask: AITask
    {
        public int BlockRadius
        {
            get; private set;
        }

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

                    if (randomPoint.X == findCircle.CenterX && randomPoint.Y == findCircle.CenterY)
                        continue;

                    bool isBlockWalkable = interiorID == Interior.Outside ? SimulationGame.World.WalkableGrid.IsBlockWalkable(randomPoint.X, randomPoint.Y) : SimulationGame.World.InteriorManager.Get(interiorID).IsBlockWalkable(randomPoint.X, randomPoint.Y);

                    if (isBlockWalkable)
                        return randomPoint;
                }

                return Point.Zero;
            });
        }

        public override void Update(GameTime gameTime)
        {
            if(IsStarted)
            {
                var movingSubject = (MovingEntity)subject;

                if (!movingSubject.IsWalking)
                {
                    if (findNextWalkablePoint == null)
                    {
                        IsFinished = true;
                    }
                    else if (findNextWalkablePoint != null && findNextWalkablePoint.IsCompleted)
                    {
                        Point destBlock = findNextWalkablePoint.Result;

                        if (wanderCircle.Contains(destBlock))
                        {
                            movingSubject.WalkTo(destBlock.X, destBlock.Y);
                        }
                        else
                        {
                            IsFinished = true;
                        }

                        findNextWalkablePoint = null;
                    }
                }
            }
        }
    }
}
