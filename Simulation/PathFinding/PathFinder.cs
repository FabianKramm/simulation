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
            return Task.Run(() => FindPathSync(startBlockX, startBlockY, endBlockX, endBlockY));
        }

        public static Task<List<GridPos>> FindPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY, Interior interior)
        {
            return Task.Run(() => FindPathSync(startBlockX, startBlockY, endBlockX, endBlockY, interior));
        }

        public static List<GridPos> FindPathSync(int startBlockX, int startBlockY, int endBlockX, int endBlockY, Interior interior = null)
        {
            ThreadingUtils.assertChildThread();

            if(interior == null)
            {
                JumpPointParam jpp = new JumpPointParam(new DynamicWalkableGrid(SimulationGame.World.WalkableGrid, startBlockX, startBlockY, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

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
