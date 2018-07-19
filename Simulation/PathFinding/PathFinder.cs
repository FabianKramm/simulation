using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulation.PathFinding
{
    public class PathFinder
    {
        // Returns a path within one interiorid, or returns the path to the next worldlink leading to the correct end interior
        public static Task<List<GridPos>> FindPath(WorldPosition startBlockPosition, WorldPosition endBlockPosition)
        {
            return Task.Run(() => findPathSync(startBlockPosition, endBlockPosition));
        }

        private static List<GridPos> findPathSync(WorldPosition startBlockPosition, WorldPosition endBlockPosition)
        {
            ThreadingUtils.assertChildThread();

            if(startBlockPosition.InteriorID != endBlockPosition.InteriorID)
            {
                var myPoint = findNextWorldLink(startBlockPosition, endBlockPosition);

                if (myPoint != null)
                {
                    Point newPoint = myPoint ?? Point.Zero;

                    endBlockPosition = endBlockPosition.Clone();
                    endBlockPosition.X = newPoint.X;
                    endBlockPosition.Y = newPoint.Y;
                    endBlockPosition.InteriorID = startBlockPosition.InteriorID;
                }
                else
                {
                    return null;
                }
            }

            if(startBlockPosition.InteriorID == Interior.Outside)
            {
                JumpPointParam jpp = new JumpPointParam(
                    new DynamicWalkableGrid(SimulationGame.World.WalkableGrid, (int)startBlockPosition.X, (int)startBlockPosition.Y, (int)endBlockPosition.X, (int)endBlockPosition.X), 
                    new GridPos((int)startBlockPosition.X, (int)startBlockPosition.Y), 
                    new GridPos((int)endBlockPosition.X, (int)endBlockPosition.Y), 
                    DiagonalMovement.OnlyWhenNoObstacles
                );

                return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
            }
            else
            {
                var interior = SimulationGame.World.InteriorManager.Get(startBlockPosition.InteriorID);
                var jpp = new JumpPointParam(
                    new InteriorGrid(interior, (int)endBlockPosition.X, (int)endBlockPosition.Y), 
                    new GridPos((int)startBlockPosition.X, (int)startBlockPosition.Y), 
                    new GridPos((int)endBlockPosition.X, (int)endBlockPosition.Y), 
                    DiagonalMovement.OnlyWhenNoObstacles
                );

                return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jpp));
            }
        }

        private static Point? findNextWorldLink(WorldPosition startBlockPos, WorldPosition endBlockPos)
        {
            if (startBlockPos.InteriorID == endBlockPos.InteriorID)
                return null;

            // We want to start at the end, because starting outside can be problematic
            if (startBlockPos.InteriorID == Interior.Outside)
            {
                WorldPosition temp = startBlockPos;
                startBlockPos = endBlockPos;
                endBlockPos = temp;
            }

            List<string> done = new List<string>() { startBlockPos.InteriorID };
            SortedList<float, WorldLink> open = new SortedList<float, WorldLink>();
            Interior startInterior = SimulationGame.World.InteriorManager.Get(startBlockPos.InteriorID);

            foreach (var worldLinkItem in startInterior.WorldLinks)
            {
                if (worldLinkItem.Value.ToInteriorID == endBlockPos.InteriorID)
                    return worldLinkItem.Value.ToBlock;
                if (worldLinkItem.Value.ToInteriorID == Interior.Outside || done.Contains(worldLinkItem.Value.ToInteriorID))
                    continue;

                open.Add(GeometryUtils.GetDiagonalDistance(startBlockPos.X, startBlockPos.Y, worldLinkItem.Value.FromBlock.X, worldLinkItem.Value.FromBlock.Y), worldLinkItem.Value);
                done.Add(worldLinkItem.Value.ToInteriorID);
            }

            while (open.Count > 0)
            {
                float distance = open.Keys[0];
                WorldLink worldLink = open.Values[0];

                open.RemoveAt(0);

                // Get neighbors
                Interior interior = SimulationGame.World.InteriorManager.Get(worldLink.ToInteriorID);

                foreach (var worldLinkItem in interior.WorldLinks)
                {
                    if (worldLinkItem.Value.ToInteriorID == endBlockPos.InteriorID)
                        return worldLinkItem.Value.ToBlock;
                    if (worldLinkItem.Value.ToInteriorID == Interior.Outside || done.Contains(worldLinkItem.Value.ToInteriorID))
                        continue;

                    open.Add(distance + GeometryUtils.GetDiagonalDistance(worldLink.ToBlock.X, worldLink.ToBlock.Y, worldLinkItem.Value.FromBlock.X, worldLinkItem.Value.FromBlock.Y), worldLinkItem.Value);
                    done.Add(worldLinkItem.Value.ToInteriorID);
                }
            }

            return null;
        }
    }
}
