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

        private static List<GridPos> findPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY)
        {
            if (SimulationGame.world.walkableGrid.IsBlockWalkable(endBlockX, endBlockY))
            {
                JumpPointParam jpp = new JumpPointParam(new DynamicGrid(SimulationGame.world.walkableGrid, startBlockX, startBlockY, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

                return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
            }

            return null;
        }
    }
}
