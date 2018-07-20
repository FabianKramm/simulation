using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Util;
using Simulation.Util.Geometry;

namespace Simulation.Game.World
{
    public class Interior: WorldPart
    {
        public static string Outside = null;

        public string ID;
        public Point Dimensions;

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            ID = Util.Util.GetUUID();
            Dimensions = dimensions;

            blockingGrid = new BlockType[dimensions.X, dimensions.Y];
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
