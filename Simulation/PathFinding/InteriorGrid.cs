using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.PathFinding
{
    class InteriorGrid: BaseGrid
    {
        private Dictionary<string, Node> nodeDir = new Dictionary<string, Node>();
        private Interior interior;

        private Rectangle interiorGridBounds;

        public InteriorGrid(Interior interior)
        {
            this.interior = interior;

            interiorGridBounds = new Rectangle(0, 0, interior.Dimensions.X, interior.Dimensions.Y);
        }

        private Node getNodeFromWalkableGrid(int blockX, int blockY)
        {
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
            return interiorGridBounds.Contains(blockX, blockY) && interior.IsBlockWalkable(blockX, blockY);
        }

        public override Node GetNodeAt(GridPos blockPos)
        {
            return GetNodeAt(blockPos.x, blockPos.y);
        }

        public override bool IsWalkableAt(GridPos blockPos)
        {
            return interiorGridBounds.Contains(blockPos.x, blockPos.y) && interior.IsBlockWalkable(blockPos.x, blockPos.y);
        }

        public override void Reset()
        {
            nodeDir = new Dictionary<string, Node>();
        }
    }
}
