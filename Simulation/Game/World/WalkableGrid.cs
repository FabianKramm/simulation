using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Hud;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

/*
 * The WalkableGrid is only used for quick pathfinding
 */
namespace Simulation.Game.World
{
    // TODO: Cleanup and save unused chunks
    public class WalkableGrid
    {
        public static Point WalkableGridBlockChunkSize = new Point(32, 32);
        public static Point WalkableGridPixelChunkSize = new Point(WalkableGridBlockChunkSize.X * World.BlockSize.X, WalkableGridBlockChunkSize.Y * World.BlockSize.Y);

        public static int WalkableGridArrayChunkCount = WalkableGridBlockChunkSize.X * WalkableGridBlockChunkSize.Y / 32;

        private TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;
        private static TimeSpan garbageCollectInterval = TimeSpan.FromSeconds(60);

        private ConcurrentDictionary<string, WalkableGridChunk> walkableGrid = new ConcurrentDictionary<string, WalkableGridChunk>();
        private NamedLock chunkLocks = new NamedLock();
        private object writeLock = new object();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void loadGridChunk(int chunkX, int chunkY)
        {
            walkableGrid[chunkX + "," + chunkY] = WorldLoader.loadWalkableGridChunk(chunkX, chunkY);
        }

        public void loadGridChunkGuarded(int chunkX, int chunkY)
        {
            var key = chunkX + "," + chunkY;

            chunkLocks.Enter(key);

            try
            {
                if (walkableGrid.ContainsKey(key) == false)
                    loadGridChunk(chunkX, chunkY);
            }
            finally
            {
                chunkLocks.Exit(key);
            }
        }

        public bool IsChunkLoaded(int chunkX, int chunkY)
        {
            var key = chunkX + "," + chunkY;

            return walkableGrid.ContainsKey(key);
        }

        public bool IsPositionWalkable(int realX, int realY)
        {
            var blockPosition = GeometryUtils.getChunkPosition(realX, realY, World.BlockSize.X, World.BlockSize.Y);

            return IsBlockWalkable(blockPosition.X, blockPosition.Y);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var key = chunkPosition.X + "," + chunkPosition.Y;
            WalkableGridChunk walkableGridChunk;

            walkableGrid.TryGetValue(key, out walkableGridChunk);

            if(walkableGridChunk == null)
            {
                chunkLocks.Enter(key);

                try
                {
                    if (walkableGrid.ContainsKey(key) == false)
                        loadGridChunk(chunkPosition.X, chunkPosition.Y);

                    walkableGridChunk = walkableGrid[key];
                }
                finally
                {
                    chunkLocks.Exit(key);
                }
            }

            return !getBit(walkableGridChunk.chunkData[arrayPosition / 32], arrayPosition % 32);
        }

        public void setBlockNotWalkable(int blockX, int blockY, bool notWalkable)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var key = chunkPosition.X + "," + chunkPosition.Y;

            chunkLocks.Enter(key);

            try
            {
                if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.Y) == false)
                    loadGridChunk(chunkPosition.X, chunkPosition.Y);

                setBit(ref walkableGrid[chunkPosition.X + "," + chunkPosition.Y].chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
            }
            finally
            {
                chunkLocks.Exit(key);
            }
        }

        public void addInteractiveObject(HitableObject hitableObject)
        {
            if (hitableObject.blockingType == BlockingType.BLOCKING)
            {
                Point blockTopLeft = GeometryUtils.getChunkPosition(hitableObject.blockingBounds.Left, hitableObject.blockingBounds.Top, World.BlockSize.X, World.BlockSize.Y);
                Point blockBottomRight = GeometryUtils.getChunkPosition(hitableObject.blockingBounds.Right - 1, hitableObject.blockingBounds.Bottom - 1, World.BlockSize.X, World.BlockSize.Y);

                for (int blockX = blockTopLeft.X; blockX <= blockBottomRight.X; blockX++)
                    for (int blockY = blockTopLeft.Y; blockY <= blockBottomRight.Y; blockY++)
                    {
                        Point walkableGridChunkPos = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

                        if (IsChunkLoaded(walkableGridChunkPos.X, walkableGridChunkPos.Y))
                        {
                            setBlockNotWalkable(blockX, blockY, true);
                        }
                    }
            }
        }

        public void removeInteractiveObject(HitableObject hitableObject)
        {
            if (hitableObject.blockingType == BlockingType.BLOCKING)
            {
                Point blockTopLeft = GeometryUtils.getChunkPosition(hitableObject.blockingBounds.Left, hitableObject.blockingBounds.Top, World.BlockSize.X, World.BlockSize.Y);
                Point blockBottomRight = GeometryUtils.getChunkPosition(hitableObject.blockingBounds.Right - 1, hitableObject.blockingBounds.Bottom - 1, World.BlockSize.X, World.BlockSize.Y);

                for (int blockX = blockTopLeft.X; blockX <= blockBottomRight.X; blockX++)
                    for (int blockY = blockTopLeft.Y; blockY <= blockBottomRight.Y; blockY++)
                    {
                        Point walkableGridChunkPos = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGrid.WalkableGridBlockChunkSize.X, WalkableGrid.WalkableGridBlockChunkSize.Y);

                        if (IsChunkLoaded(walkableGridChunkPos.X, walkableGridChunkPos.Y))
                        {
                            Point worldGridChunkPos = GeometryUtils.getChunkPosition(blockX, blockY, World.WorldChunkBlockSize.X, World.WorldChunkBlockSize.Y);
                            WorldGridChunk worldGridChunk = SimulationGame.world.getWorldGridChunk(worldGridChunkPos.X, worldGridChunkPos.Y);
                            BlockType blockType = worldGridChunk.getBlockType(blockX, blockY);

                            if (CollisionUtils.getBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                                continue;

                            var found = false;

                            foreach (HitableObject interactiveObject in worldGridChunk.interactiveObjects)
                            {
                                if(blockX == 3 && blockY == -2 && interactiveObject.ID == "ca1bb72b-ca15-4751-a58d-65c1fc74ea5f")
                                {
                                    var a = 1;
                                }

                                if (hitableObject.blockingType == BlockingType.BLOCKING && interactiveObject.blockingBounds.Intersects(new Rectangle(blockX * World.BlockSize.X, blockY * World.BlockSize.Y, World.BlockSize.X, World.BlockSize.Y)))
                                {
                                    found = true;
                                    break;
                                }
                            }
                                
                            if (!found)
                            {
                                setBlockNotWalkable(blockX, blockY, false);
                            }
                        }
                    }
            }
        }

        private void garbageCollectWalkableGrid()
        {
            ThreadingUtils.assertMainThread();
            int chunksUnloaded = 0;

            foreach(var walkableGridChunkItem in walkableGrid)
            {
                var key = walkableGridChunkItem.Key;
                chunkLocks.Enter(key);

                try
                {
                    var found = false;

                    foreach (var durableEntity in SimulationGame.world.durableEntities)
                    {
                        if (walkableGridChunkItem.Value.realChunkBounds.Intersects(durableEntity.Value.preloadedWorldGridChunkPixelBounds))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                    {
                        WalkableGridChunk removedItem;

                        walkableGrid.TryRemove(key, out removedItem);

                        // Save async
                        chunksUnloaded++;
                    }
                }
                finally
                {
                    chunkLocks.Exit(key);
                }
            }

            if(chunksUnloaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + chunksUnloaded + " walkable grid chunks");
            }
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastGarbageCollect += gameTime.ElapsedGameTime;

            if (timeSinceLastGarbageCollect > garbageCollectInterval)
            {
                timeSinceLastGarbageCollect = TimeSpan.Zero;
                garbageCollectWalkableGrid();
            }
        }

        // Don't care about concurrency
        public static void setBlockNotWalkableInChunk(WalkableGridChunk chunk, int blockX, int blockY, bool notWalkable)
        {
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            setBit(ref chunk.chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
        }

        private static bool getBit(UInt32 x, int bitnum)
        {
            return (x & (1 << bitnum)) != 0;
        }

        private static void setBit(ref UInt32 x, int bitnum, bool val)
        {
            if (val)
                x |= (UInt32)(1 << bitnum);
            else
                x &= ~(UInt32)(1 << bitnum);
        }
    }
}
