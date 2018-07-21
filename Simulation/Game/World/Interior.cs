using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Util;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System.Diagnostics;

namespace Simulation.Game.World
{
    public class Interior: WorldPart
    {
        public static string Outside = null;
        public string ID;

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            ID = Util.Util.GetUUID();
            Dimensions = dimensions;
            blockingGrid = new BlockType[dimensions.X, dimensions.Y];
        }

        public override BlockType GetBlockType(int blockX, int blockY)
        {
            if (blockX < 0 || blockX >= Dimensions.X || blockY < 0 || blockY >= Dimensions.Y)
                return BlockType.NONE;

            return blockingGrid[blockX, blockY];
        }

        public override void SetBlockType(int blockX, int blockY, BlockType blockType)
        {
            Debug.Assert(Connected == false, "Cannot set block type, when already connected to world!");

            blockingGrid[blockX, blockY] = blockType;
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            ThreadingUtils.assertChildThread();

            if(CollisionUtils.GetBlockingTypeFromBlock(GetBlockType(blockX, blockY)) == BlockingType.BLOCKING)
            {
                return false;
            }

            Rect blockBounds = new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            if (ContainedObjects != null)
            {
                lock(ContainedObjects)
                {
                    foreach (var containedObject in ContainedObjects)
                    {
                        lock(containedObject)
                        {
                            if (containedObject.BlockingType == BlockingType.BLOCKING && containedObject.BlockingBounds.Intersects(blockBounds))
                                return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
