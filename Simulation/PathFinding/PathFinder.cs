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
                JumpPointParam jpp = new JumpPointParam(new DynamicWalkableGrid(SimulationGame.World.walkableGrid, startBlockX, startBlockY, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

                return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
            }
            else
            {
                JumpPointParam jpp = new JumpPointParam(new InteriorGrid(interior, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

                return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
            }
        }
    }
}
