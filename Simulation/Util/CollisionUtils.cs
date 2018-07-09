using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.World;

namespace Simulation.Util
{
    public class CollisionUtils
    {
        public static bool canMove(HitableObject origin, Rectangle rect)
        {
            ThreadingUtils.assertMainThread();

            if(origin.InteriorID == Interior.Outside)
            {
                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.getChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.getChunkPosition(rect.Right - 1, rect.Bottom - 1, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        Point chunkPos = GeometryUtils.getChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.getWorldGridChunk(chunkPos.X, chunkPos.Y);

                        BlockType blockType = worldGridChunk.getBlockType(blockX, blockY);

                        if (CollisionUtils.getBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            return false;
                    }

                // Check collision with interactive && contained objects
                Point chunkTopLeft = GeometryUtils.getChunkPosition(rect.Left, rect.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.getChunkPosition(rect.Right - 1, rect.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.getWorldGridChunk(chunkX, chunkY);

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.OverlappingObjects)
                                if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject != origin && hitableObject.blockingBounds.Intersects(rect))
                                    return false;

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.ContainedObjects)
                                if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject != origin && hitableObject.blockingBounds.Intersects(rect))
                                    return false;
                    }

                return true;
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.GetInterior(origin.InteriorID);

                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.getChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.getChunkPosition(rect.Right - 1, rect.Bottom - 1, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        BlockType blockType = interior.GetBlockType(blockX, blockY);

                        if (CollisionUtils.getBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            return false;
                    }

                foreach (HitableObject hitableObject in interior.ContainedObjects)
                    if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject != origin && hitableObject.blockingBounds.Intersects(rect))
                        return false;

                return true;
            }
        }

        public static BlockingType getBlockingTypeFromBlock(BlockType blockType)
        {
            switch(blockType)
            {
                case BlockType.NONE:
                case BlockType.GRASS_WATERHOLE:
                    return BlockingType.BLOCKING;
                default:
                    return BlockingType.NOT_BLOCKING;
            }
        }
    }
}
