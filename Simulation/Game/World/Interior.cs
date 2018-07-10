using Microsoft.Xna.Framework;
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

        // This object is needed for restricting access to the contained objects
        private object ContainedObjectsLock = new object();

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            ID = Util.Util.getUUID();
            Dimensions = dimensions;

            blockingGrid = new BlockType[dimensions.X, dimensions.Y];
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            if(CollisionUtils.GetBlockingTypeFromBlock(GetBlockType(blockX, blockY)) == BlockingType.BLOCKING)
            {
                return false;
            }

            Rect blockBounds = new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            // Check Contained Objects
            lock(ContainedObjectsLock)
            {
                if (ContainedObjects != null)
                    foreach (var containedObject in ContainedObjects)
                        if (containedObject.BlockingType == BlockingType.BLOCKING && containedObject.BlockingBounds.Intersects(blockBounds))
                            return false;
            }

            return true;
        }

        public override void AddContainedObject(HitableObject containedObject)
        {
            lock(ContainedObjectsLock)
            {
                base.AddContainedObject(containedObject);
            }
        }

        public override void RemoveContainedObject(HitableObject containedObject)
        {
            lock(ContainedObjectsLock)
            {
                base.RemoveContainedObject(containedObject);
            }
        }
    }
}
