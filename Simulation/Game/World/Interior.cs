using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public class Interior
    {
        public static string Outside = null;

        public string ID;

        private BlockType[,] blockingGrid;
        public Point Dimensions;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> ContainedObjects;
        private object ContainedObjectsLock = new object();

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        // These objects are used to connect this interior to the world
        public Dictionary<string, WorldLink> WorldLinks;

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            ID = Util.Util.getUUID();
            Dimensions = dimensions;

            blockingGrid = new BlockType[dimensions.X, dimensions.Y];
        }

        public void SetBlockType(int blockX, int blockY, BlockType blockType)
        {
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public BlockType GetBlockType(int blockX, int blockY)
        {
            if (blockX < 0 || blockX >= Dimensions.X)
                return BlockType.NONE;
            if (blockY < 0 || blockY >= Dimensions.Y)
                return BlockType.NONE;

            return blockingGrid[blockX, blockY];
        }

        public void AddWorldLink(WorldLink worldLink)
        {
            string key = worldLink.FromBlock.X + "," + worldLink.FromBlock.Y;

            if (WorldLinks == null)
                WorldLinks = new Dictionary<string, WorldLink>();

            if (WorldLinks.ContainsKey(key) == false)
                WorldLinks[key] = worldLink;
        }

        public void RemoveWorldLink(WorldLink worldLink)
        {
            string key = worldLink.FromBlock.X + "," + worldLink.FromBlock.Y;

            if (WorldLinks != null)
            {
                WorldLinks.Remove(key);

                if (WorldLinks.Count == 0)
                {
                    WorldLinks = null;
                }
            }
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            if(CollisionUtils.getBlockingTypeFromBlock(GetBlockType(blockX, blockY)) == BlockingType.BLOCKING)
            {
                return false;
            }

            Rect blockBounds = new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            // Check Contained Objects
            lock(ContainedObjectsLock)
            {
                if (ContainedObjects != null)
                    foreach (HitableObject containedObject in ContainedObjects)
                        if (containedObject.BlockingType == BlockingType.BLOCKING && containedObject.BlockingBounds.Intersects(blockBounds))
                            return false;
            }

            return true;
        }

        public void AddContainedObject(HitableObject containedObject)
        {
            lock(ContainedObjectsLock)
            {
                if (ContainedObjects == null)
                    ContainedObjects = new List<HitableObject>();

                if (ContainedObjects.Contains(containedObject) == false)
                    ContainedObjects.Add(containedObject);
            }
        }

        public void RemoveContainedObject(HitableObject interactiveObject)
        {
            lock(ContainedObjectsLock)
            {
                if (ContainedObjects != null)
                {
                    ContainedObjects.Remove(interactiveObject);

                    if (ContainedObjects.Count == 0)
                    {
                        ContainedObjects = null;
                    }
                }
            }

        }

        public void AddAmbientObject(AmbientObject ambientObject)
        {
            if (AmbientObjects == null)
                AmbientObjects = new List<AmbientObject>();

            if (AmbientObjects.Contains(ambientObject) == false)
                AmbientObjects.Add(ambientObject);
        }

        public void RemoveAmbientObject(AmbientObject ambientObject)
        {
            if (AmbientObjects != null)
            {
                AmbientObjects.Remove(ambientObject);

                if (AmbientObjects.Count == 0)
                {
                    AmbientObjects = null;
                }
            }
        }
    }
}
