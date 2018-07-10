using Microsoft.Xna.Framework;
using Simulation.Game.Generator;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI
{
    public class WanderAI: BaseAI
    {
        public int BlockRadius
        {
            get; private set;
        }

        private Circle blockCircle;
        private Task<Point> findNextWalkablePoint;

        // From JSON
        private WanderAI(MovingEntity movingEntity): base(movingEntity) { }

        public WanderAI(MovingEntity movingEntity, int blockRadius): base(movingEntity)
        {
            BlockRadius = blockRadius;

            blockCircle = new Circle(movingEntity.BlockPosition.X, movingEntity.BlockPosition.Y, blockRadius);
        }

        private Task<Point> getNextPoint()
        {
            string interiorID = Entity.InteriorID;
            Circle findCircle = blockCircle; // Clone circle to avoid concurrency problems
            Random random = new Random();

            return Task.Run(() =>
            {
                for(int i=0;i<100;i++)
                {
                    Point randomPoint = GeneratorUtils.GetRandomPointInCircle(random, findCircle);

                    if (randomPoint.X == findCircle.CenterX && randomPoint.Y == findCircle.CenterY)
                        continue;

                    bool isBlockWalkable = interiorID == Interior.Outside ? SimulationGame.World.walkableGrid.IsBlockWalkable(randomPoint.X, randomPoint.Y) : SimulationGame.World.InteriorManager.GetInterior(interiorID).IsBlockWalkable(randomPoint.X, randomPoint.Y);

                    if(isBlockWalkable)
                        return randomPoint;
                }

                return Point.Zero;
            });
        }

        // This is only called when we should wander in a new area and not when the entity walks
        public void UpdateBlockPosition(Vector2 newPosition)
        {
            blockCircle = new Circle(Entity.BlockPosition.X, Entity.BlockPosition.Y, BlockRadius);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Entity.IsWalking && findNextWalkablePoint == null)
            {
                findNextWalkablePoint = getNextPoint();
            }

            if (findNextWalkablePoint != null && findNextWalkablePoint.IsCompleted)
            {
                Point destBlock = findNextWalkablePoint.Result;

                if (!Entity.IsWalking && blockCircle.Contains(destBlock))
                {
                    Entity.WalkTo(destBlock.X, destBlock.Y);
                }

                findNextWalkablePoint = null;
            }
        }
    }
}
