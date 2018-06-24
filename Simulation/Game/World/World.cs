using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Base.Entity;
using Simulation.Game.Hud;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public class World
    {
        public static Point BlockSize = new Point(32, 32);

        public static Point WorldChunkBlockSize = new Point(32, 32); // 32 * 32 BlockSize
        public static Point WorldChunkPixelSize = new Point(WorldChunkBlockSize.X * BlockSize.X, WorldChunkBlockSize.Y * BlockSize.Y);

        public static int RenderOuterBlockRange = 3;

        private Dictionary<string, WorldGridChunk> worldGrid = new Dictionary<string, WorldGridChunk>();
       
        private Dictionary<string, HitableObject> interactiveObjects;
        private Dictionary<string, DrawableObject> effects;

        private Dictionary<string, DurableEntity> durableEntities = new Dictionary<string, DurableEntity>();

        private NamedLock chunkLocks = new NamedLock();
        private ConcurrentQueue<WorldGridChunk> chunksLoaded = new ConcurrentQueue<WorldGridChunk>();

        private WalkableGrid walkableGrid = new WalkableGrid();

        private TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;
        private static TimeSpan garbageCollectInterval = TimeSpan.FromSeconds(30);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isWorldGridChunkLoaded(int chunkX, int chunkY)
        {
            return worldGrid.ContainsKey(chunkX + "," + chunkY);
        }

        public void loadGridChunkAsync(int chunkX, int chunkY)
        {
            if(isWorldGridChunkLoaded(chunkX, chunkY) == false)
            {
                Task.Run(() =>
                {
                    string chunkKey = chunkX + "," + chunkY;

                    bool couldLock = chunkLocks.TryEnter(chunkKey);

                    if (!couldLock)
                    {
                        return;
                    }

                    try
                    {
                        foreach(WorldGridChunk worldGridChunk in chunksLoaded)
                        {
                            Point chunkPosition = GeometryUtils.getChunkPosition(worldGridChunk.realChunkBounds.X, worldGridChunk.realChunkBounds.Y, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

                            if (chunkPosition.X == chunkX && chunkPosition.Y == chunkY)
                            {
                                return;
                            }
                        }

                        chunksLoaded.Enqueue(WorldLoader.loadWorldGridChunk(chunkX, chunkY));
                    }
                    finally
                    {
                        chunkLocks.Exit(chunkKey);
                    }
                });
            }
        }

        public void applyLoadedChunks()
        {
            ThreadingUtils.checkIfMainThread();

            int amountChunksLoaded = 0;

            while (chunksLoaded.IsEmpty == false)
            {
                WorldGridChunk worldGridChunk;

                bool dequeued = chunksLoaded.TryDequeue(out worldGridChunk);

                if(!dequeued)
                {
                    break;
                }

                int chunkX = (worldGridChunk.realChunkBounds.X / WorldChunkPixelSize.X);
                int chunkY = (worldGridChunk.realChunkBounds.Y / WorldChunkPixelSize.Y);
                string chunkKey = chunkX + "," + chunkY;

                if (worldGrid.ContainsKey(chunkKey) == false)
                {
                    worldGrid[chunkKey] = worldGridChunk;

                    onChunkLoaded(worldGridChunk, chunkX, chunkY);

                    amountChunksLoaded++;
                }
            }

            if(amountChunksLoaded > 0)
            {
                GameConsole.WriteLine(amountChunksLoaded + " chunks preloaded");
            }
        }

        public WorldGridChunk getWorldGridChunk(int chunkX, int chunkY)
        {
            ThreadingUtils.checkIfMainThread();

            var chunkKey = chunkX + "," + chunkY;

            if(worldGrid.ContainsKey(chunkKey) == false)
            {
                worldGrid[chunkKey] = WorldLoader.loadWorldGridChunk(chunkX, chunkY);

                onChunkLoaded(worldGrid[chunkKey], chunkX, chunkY);
            }

            return worldGrid[chunkKey];
        }

        /*
            This function is executed when a new chunk was loaded and objects from other chunks could overlap with this chunk
         */
        public void onChunkLoaded(WorldGridChunk chunk, int chunkX, int chunkY)
        {
            ThreadingUtils.checkIfMainThread();

            foreach (DrawableObject drawableObject in chunk.ambientObjects)
            {
                if(drawableObject is HitableObject)
                {
                    chunk.addHitableObject((HitableObject)drawableObject);
                }
            }

            for(int i=-1;i<=1;i++)
                for(int j=-1;j<1;j++)
                {
                    if (i == 0 && j == 0) continue;

                    string neighborKey = (chunkX + i) + "," + (chunkY + j);

                    if (worldGrid.ContainsKey(neighborKey) == true)
                    {
                        foreach (HitableObject hitableObject in chunk.interactiveObjects)
                        {
                            if (hitableObject.unionBounds.Intersects(worldGrid[neighborKey].realChunkBounds))
                            {
                                worldGrid[neighborKey].addHitableObject(hitableObject);
                            }
                        }

                        foreach (HitableObject hitableObject in worldGrid[neighborKey].interactiveObjects)
                        {
                            if (hitableObject.unionBounds.Intersects(chunk.realChunkBounds))
                            {
                                chunk.addHitableObject(hitableObject);
                            }
                        }
                    }
                }
        }

        public bool canMove(Rectangle rect)
        {
            ThreadingUtils.checkIfMainThread();

            // Check if blocks are of type blocking
            Point topLeft = GeometryUtils.getChunkPosition(rect.Left, rect.Top, BlockSize.X, BlockSize.Y);
            Point bottomRight = GeometryUtils.getChunkPosition(rect.Right, rect.Bottom, BlockSize.X, BlockSize.Y);

            for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                {
                    Point chunkPos = GeometryUtils.getChunkPosition(blockX, blockY, WorldChunkBlockSize.X, WorldChunkBlockSize.Y);
                    WorldGridChunk worldGridChunk = getWorldGridChunk(chunkPos.X, chunkPos.Y);

                    BlockType blockType = worldGridChunk.getBlockType(blockX, blockY);

                    if (CollisionUtils.getBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                        return false;
                }


            // Check collision with interactive objects
            Point chunkTopLeft = GeometryUtils.getChunkPosition(rect.Left, rect.Top, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.getChunkPosition(rect.Right, rect.Bottom, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    WorldGridChunk worldGridChunk = getWorldGridChunk(chunkX, chunkY);

                    if (worldGridChunk.interactiveObjects != null) 
                        foreach (HitableObject hitableObject in worldGridChunk.interactiveObjects)
                            if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject.blockingBounds.Intersects(rect))
                                return false;
                }

            return true;
        }

        public void addHitableObject(HitableObject hitableObject)
        {
            ThreadingUtils.checkIfMainThread();

            Point chunkTopLeft = GeometryUtils.getChunkPosition(hitableObject.unionBounds.Left, hitableObject.unionBounds.Top, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.getChunkPosition(hitableObject.unionBounds.Right, hitableObject.unionBounds.Bottom, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if(isWorldGridChunkLoaded(chunkX, chunkY))
                    {
                        getWorldGridChunk(chunkX, chunkY).addHitableObject(hitableObject);
                    }
                }
        }

        public void removeHitableObject(HitableObject hitableObject)
        {
            ThreadingUtils.checkIfMainThread();

            Point chunkTopLeft = GeometryUtils.getChunkPosition(hitableObject.unionBounds.Left, hitableObject.unionBounds.Top, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.getChunkPosition(hitableObject.unionBounds.Right, hitableObject.unionBounds.Bottom, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if (isWorldGridChunkLoaded(chunkX, chunkY))
                    {
                        getWorldGridChunk(chunkX, chunkY).removeHitableObject(hitableObject);
                    }
                }
        }

        public void addDurableEntity(DurableEntity durableEntity)
        {
            ThreadingUtils.checkIfMainThread();

            if (durableEntities.ContainsKey(durableEntity.ID) == false)
                durableEntities[durableEntity.ID] = durableEntity;
        }

        private void garbageCollectChunks()
        {
            ThreadingUtils.checkIfMainThread();
            List<string> deleteList = new List<string>();

            foreach (var chunk in worldGrid)
            {
                var found = false;

                foreach (var durableEntity in durableEntities)
                {
                    if (chunk.Value.realChunkBounds.Intersects(durableEntity.Value.preloadedWorldGridChunkPixelBounds))
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    deleteList.Add(chunk.Key);
                }
            }

            foreach(var key in deleteList)
            {
                worldGrid.Remove(key);

                // Save async
            }

            GameConsole.WriteLine("Garbage Collector unloaded " + deleteList.Count + " chunks");
        }

        public void Update(GameTime gameTime)
        {
            applyLoadedChunks();

            timeSinceLastGarbageCollect += gameTime.ElapsedGameTime;

            if(timeSinceLastGarbageCollect > garbageCollectInterval)
            {
                timeSinceLastGarbageCollect = TimeSpan.Zero;
                garbageCollectChunks();
            }
        }
    }
}
