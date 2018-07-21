using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.PathFinding
{
    public class DynamicWalkableGrid: BaseGrid
    {
        private static int outsideSearchDistanceAllowed = 32;

        private Dictionary<ulong, Node> nodeDir = new Dictionary<ulong, Node>();
        private WalkableGrid walkableGrid;

        private Rect dynamicGridBounds;
        private Point destination;

        public DynamicWalkableGrid(WalkableGrid walkableGrid, int startBlockX, int startBlockY, int endBlockX, int endBlockY)
        {
            this.walkableGrid = walkableGrid;

            destination = new Point(endBlockX, endBlockY);

            startBlockX -= outsideSearchDistanceAllowed;
            startBlockY -= outsideSearchDistanceAllowed;
            endBlockX += outsideSearchDistanceAllowed;
            endBlockY += outsideSearchDistanceAllowed;

            dynamicGridBounds = new Rect(startBlockX, startBlockY, Math.Abs(endBlockX - startBlockX), Math.Abs(endBlockY - startBlockY));
        }

        private Node getNodeFromWalkableGrid(int blockX, int blockY)
        {
            if (blockX == destination.X && blockY == destination.Y)
                return new Node(blockX, blockY, true); // End Block Is always walkable

            return new Node(blockX, blockY, walkableGrid.IsBlockWalkable(blockX, blockY));
        }

        public override Node GetNodeAt(int blockX, int blockY)
        {
            ulong key = GeometryUtils.ConvertPointToLong(blockX, blockY);

            if (!nodeDir.ContainsKey(key))
            {
                nodeDir[key] = getNodeFromWalkableGrid(blockX, blockY);
            }

            return nodeDir[key];
        }

        public override bool IsWalkableAt(int blockX, int blockY)
        {
            if (blockX == destination.X && blockY == destination.Y)
                return true; // End Block Is always walkable

            return dynamicGridBounds.Contains(blockX, blockY) && walkableGrid.IsBlockWalkable(blockX, blockY);
        }

        public override Node GetNodeAt(GridPos blockPos)
        {
            return GetNodeAt(blockPos.x, blockPos.y);
        }

        public override bool IsWalkableAt(GridPos blockPos)
        {
            if (blockPos.x == destination.X && blockPos.y == destination.Y)
                return true; // End Block Is always walkable

            return dynamicGridBounds.Contains(blockPos.x, blockPos.y) && walkableGrid.IsBlockWalkable(blockPos.x, blockPos.y);
        }

        public override void Reset()
        {
            nodeDir = new Dictionary<ulong, Node>();
        }
    }
}
