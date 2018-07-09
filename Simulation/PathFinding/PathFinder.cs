using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulation.PathFinding
{
    public class PathFinder
    {
        public static Task<List<GridPos>> FindPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY)
        {
            return Task.Run(() => findPath(startBlockX, startBlockY, endBlockX, endBlockY));
        }

        public static Task<List<GridPos>> FindPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY, Interior interior)
        {
            return Task.Run(() => findPath(startBlockX, startBlockY, endBlockX, endBlockY, interior));
        }

        private static List<GridPos> findPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY, Interior interior = null)
        {
            if(interior == null)
            {
                if (SimulationGame.World.walkableGrid.IsBlockWalkable(endBlockX, endBlockY))
                {
                    JumpPointParam jpp = new JumpPointParam(new DynamicWalkableGrid(SimulationGame.World.walkableGrid, startBlockX, startBlockY, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

                    return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
                }
            }
            else
            {
                if (CollisionUtils.getBlockingTypeFromBlock(interior.GetBlockType(endBlockX, endBlockY)) != BlockingType.BLOCKING)
                {
                    JumpPointParam jpp = new JumpPointParam(new InteriorGrid(interior), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

                    return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
                }
            }

            return null;
        }
    }
}
