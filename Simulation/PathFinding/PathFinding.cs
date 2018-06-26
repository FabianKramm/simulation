using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.PathFinding
{
    public class PathFinding
    {
        private WalkableGrid walkableGrid;

        public PathFinding(WalkableGrid walkableGrid)
        {
            this.walkableGrid = walkableGrid;
        }

        public List<GridPos> FindPath(int startBlockX, int startBlockY, int endBlockX, int endBlockY)
        {
            ThreadingUtils.assertChildThread();

            JumpPointParam jpp = new JumpPointParam(new DynamicGrid(walkableGrid, startBlockX, startBlockY, endBlockX, endBlockY), new GridPos(startBlockX, startBlockY), new GridPos(endBlockX, endBlockY), DiagonalMovement.OnlyWhenNoObstacles);

            return JumpPointFinder.FindPath(jpp);
        }
    }
}
