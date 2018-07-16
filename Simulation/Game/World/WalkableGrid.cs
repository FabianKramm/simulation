using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Hud;
using Simulation.Game.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Util.Geometry;

/*
 * The WalkableGrid is only used for quick pathfinding
 */
namespace Simulation.Game.World
{
    // TODO: Cleanup and save unused chunks
    public class WalkableGrid
    {
        public static Point WalkableGridBlockChunkSize = new Point(32, 32);
        public static Point WalkableGridPixelChunkSize = new Point(WalkableGridBlockChunkSize.X * WorldGrid.BlockSize.X, WalkableGridBlockChunkSize.Y * WorldGrid.BlockSize.Y);

        public static int WalkableGridArrayChunkCount = WalkableGridBlockChunkSize.X * WalkableGridBlockChunkSize.Y / 32;

        private TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;
        private static TimeSpan garbageCollectInterval = TimeSpan.FromSeconds(30);

        private ConcurrentDictionary<string, WalkableGridChunk> walkableGrid = new ConcurrentDictionary<string, WalkableGridChunk>();
        private NamedLock chunkLocks = new NamedLock();
        private object writeLock = new object();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void loadGridChunk(int chunkX, int chunkY)
        {
            walkableGrid[chunkX + "," + chunkY] = WorldLoader.LoadWalkableGridChunk(chunkX, chunkY);
        }

        public int getLoadedChunkAmount()
        {
            return walkableGrid.Count;
        }

        public void saveGridChunkAsync(int chunkX, int chunkY, WalkableGridChunk chunk)
        {
            Task.Run(() =>
            {
                var key = chunkX + "," + chunkY;

                chunkLocks.Enter(key);

                try
                {
                    WorldLoader.SaveWalkableGridChunk(chunkX, chunkY, chunk);
                }
                finally
                {
                    chunkLocks.Exit(key);
                }
            });
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
            var blockPosition = GeometryUtils.GetChunkPosition(realX, realY, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            return IsBlockWalkable(blockPosition.X, blockPosition.Y);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var chunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.GetIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
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
            var chunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.GetIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
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

        public void BlockRect(Rect blockingBounds)
        {
            Point blockTopLeft = GeometryUtils.GetChunkPosition(blockingBounds.Left, blockingBounds.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point blockBottomRight = GeometryUtils.GetChunkPosition(blockingBounds.Right, blockingBounds.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            for (int blockX = blockTopLeft.X; blockX <= blockBottomRight.X; blockX++)
                for (int blockY = blockTopLeft.Y; blockY <= blockBottomRight.Y; blockY++)
                {
                    Point walkableGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

                    if (IsChunkLoaded(walkableGridChunkPos.X, walkableGridChunkPos.Y))
                    {
                        setBlockNotWalkable(blockX, blockY, true);
                    }
                }
        }

        public void UnblockRect(Rect blockingBounds)
        {
            Point blockTopLeft = GeometryUtils.GetChunkPosition(blockingBounds.Left, blockingBounds.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point blockBottomRight = GeometryUtils.GetChunkPosition(blockingBounds.Right, blockingBounds.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            for (int blockX = blockTopLeft.X; blockX <= blockBottomRight.X; blockX++)
                for (int blockY = blockTopLeft.Y; blockY <= blockBottomRight.Y; blockY++)
                {
                    Point walkableGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

                    if (IsChunkLoaded(walkableGridChunkPos.X, walkableGridChunkPos.Y))
                    {
                        Point worldGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunk(worldGridChunkPos.X, worldGridChunkPos.Y);
                        BlockType blockType = worldGridChunk.GetBlockType(blockX, blockY);

                        if (CollisionUtils.GetBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            continue;

                        var found = false;

                        foreach (HitableObject interactiveObject in worldGridChunk.OverlappingObjects)
                            if (interactiveObject.BlockingType == BlockingType.BLOCKING && interactiveObject.BlockingBounds.Intersects(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
                            {
                                found = true;
                                break;
                            }
                                
                        if (!found)
                        {
                            setBlockNotWalkable(blockX, blockY, false);
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

                    foreach (var durableEntity in SimulationGame.World.DurableEntities)
                    {
                        if (durableEntity.Value.InteriorID == Interior.Outside && walkableGridChunkItem.Value.realChunkBounds.Intersects(durableEntity.Value.PreloadedWorldGridChunkPixelBounds))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                    {
                        WalkableGridChunk removedItem;

                        string[] pos = key.Split(',');

                        walkableGrid.TryRemove(key, out removedItem);

                        // Save async
                        saveGridChunkAsync(Int32.Parse(pos[0]), Int32.Parse(pos[1]), removedItem);

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
            var arrayPosition = GeometryUtils.GetIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

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
