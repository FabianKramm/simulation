using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.PathFinding
{
    class InteriorGrid: BaseGrid
    {
        private Dictionary<string, Node> nodeDir = new Dictionary<string, Node>();
        private Interior interior;

        private Rect interiorGridBounds;
        private Point destination;

        public InteriorGrid(Interior interior, int endX, int endY)
        {
            this.interior = interior;

            destination = new Point(endX, endY);

            interiorGridBounds = new Rect(0, 0, interior.Dimensions.X, interior.Dimensions.Y);
        }

        private Node getNodeFromWalkableGrid(int blockX, int blockY)
        {
            if (blockX == destination.X && blockY == destination.Y) return new Node(blockX, blockY, true); // End Block Is always walkable

            return new Node(blockX, blockY, interior.IsBlockWalkable(blockX, blockY));
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
            if (blockX == destination.X && blockY == destination.Y) return true; // End Block Is always walkable

            return interiorGridBounds.Contains(blockX, blockY) && interior.IsBlockWalkable(blockX, blockY);
        }

        public override Node GetNodeAt(GridPos blockPos)
        {
            return GetNodeAt(blockPos.x, blockPos.y);
        }

        public override bool IsWalkableAt(GridPos blockPos)
        {
            if (blockPos.x == destination.X && blockPos.y == destination.Y) return true; // End Block Is always walkable

            return interiorGridBounds.Contains(blockPos.x, blockPos.y) && interior.IsBlockWalkable(blockPos.x, blockPos.y);
        }

        public override void Reset()
        {
            nodeDir = new Dictionary<string, Node>();
        }
    }
}
