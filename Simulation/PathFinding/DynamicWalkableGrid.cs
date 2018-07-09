using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using System;
using System.Collections.Generic;

namespace Simulation.PathFinding
{
    public class DynamicWalkableGrid: BaseGrid
    {
        private static int outsideSearchDistanceAllowed = 32;

        private Dictionary<string, Node> nodeDir = new Dictionary<string, Node>();
        private WalkableGrid walkableGrid;

        private Rectangle dynamicGridBounds;

        public DynamicWalkableGrid(WalkableGrid walkableGrid, int startBlockX, int startBlockY, int endBlockX, int endBlockY)
        {
            this.walkableGrid = walkableGrid;

            startBlockX -= outsideSearchDistanceAllowed;
            startBlockY -= outsideSearchDistanceAllowed;
            endBlockX += outsideSearchDistanceAllowed;
            endBlockY += outsideSearchDistanceAllowed;

            dynamicGridBounds = new Rectangle(startBlockX, startBlockY, Math.Abs(endBlockX - startBlockX), Math.Abs(endBlockY - startBlockY));
        }

        private Node getNodeFromWalkableGrid(int blockX, int blockY)
        {
            return new Node(blockX, blockY, walkableGrid.IsBlockWalkable(blockX, blockY));
        }

        public override Node GetNodeAt(int blockX, int blockY)
        {
            string key = blockX + "," + blockY;

            if (!nodeDir.ContainsKey(key))
            {
                nodeDir[key] = getNodeFromWalkableGrid(blockX, blockY);
            }

            return nodeDir[key];
        }

        public override bool IsWalkableAt(int blockX, int blockY)
        {
            return dynamicGridBounds.Contains(blockX, blockY) && walkableGrid.IsBlockWalkable(blockX, blockY);
        }

        public override Node GetNodeAt(GridPos blockPos)
        {
            return GetNodeAt(blockPos.x, blockPos.y);
        }

        public override bool IsWalkableAt(GridPos blockPos)
        {
            return dynamicGridBounds.Contains(blockPos.x, blockPos.y) && walkableGrid.IsBlockWalkable(blockPos.x, blockPos.y);
        }

        public override void Reset()
        {
            nodeDir = new Dictionary<string, Node>();
        }
    }
}
