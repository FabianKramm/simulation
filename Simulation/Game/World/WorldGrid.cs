﻿using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Base.Entity;
using Simulation.Game.Hud;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public class WorldGrid
    {
        public static readonly Point BlockSize = new Point(32, 32);
        public static readonly Point WorldChunkBlockSize = new Point(32, 32); // 32 * 32 BlockSize
        public static readonly Point WorldChunkPixelSize = new Point(WorldChunkBlockSize.X * BlockSize.X, WorldChunkBlockSize.Y * BlockSize.Y);
        public static readonly int RenderOuterBlockRange = 3;

        public WalkableGrid walkableGrid { get; private set; } = new WalkableGrid();
        private Dictionary<string, WorldGridChunk> worldGrid = new Dictionary<string, WorldGridChunk>();
        public InteriorManager InteriorManager = new InteriorManager();

        private NamedLock chunkLocks = new NamedLock();
        private ConcurrentQueue<WorldGridChunk> worldGridChunksLoaded = new ConcurrentQueue<WorldGridChunk>();

        private TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;
        private static TimeSpan garbageCollectInterval = TimeSpan.FromSeconds(20);

        public Dictionary<string, DrawableObject> effects;
        public Dictionary<string, DurableEntity> durableEntities = new Dictionary<string, DurableEntity>();

        public int getLoadedChunkAmount()
        {
            return worldGrid.Count;
        }

        private WorldGridChunk loadWorldGridChunk(int chunkX, int chunkY)
        {
            if(Thread.CurrentThread.ManagedThreadId == 1)
            {
                GameConsole.WriteLine("ChunkLoading", chunkX + "," + chunkY + " loaded in main thread");
            }

            var walkableGridChunkPosition = GeometryUtils.GetChunkPosition(chunkX * WorldChunkPixelSize.X, chunkY * WorldChunkPixelSize.Y, WalkableGrid.WalkableGridPixelChunkSize.X, WalkableGrid.WalkableGridPixelChunkSize.Y);

            walkableGrid.loadGridChunkGuarded(walkableGridChunkPosition.X, walkableGridChunkPosition.Y);

            return WorldLoader.loadWorldGridChunk(chunkX, chunkY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool isWorldGridChunkLoaded(int chunkX, int chunkY)
        {
            return worldGrid.ContainsKey(chunkX + "," + chunkY);
        }

        public void saveWorldGridChunkAsync(int chunkX, int chunkY, WorldGridChunk chunk)
        {
            Task.Run(() =>
            {
                string chunkKey = chunkX + "," + chunkY;

                chunkLocks.Enter(chunkKey);

                try
                {
                    WorldLoader.saveWorldGridChunk(chunkX, chunkY, chunk);
                }
                finally
                {
                    chunkLocks.Exit(chunkKey);
                }
            });
        }

        public void loadWorldGridChunkAsync(int chunkX, int chunkY)
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
                        // Check if already in queue
                        foreach(WorldGridChunk worldGridChunk in worldGridChunksLoaded)
                        {
                            Point chunkPosition = GeometryUtils.GetChunkPosition(worldGridChunk.RealChunkBounds.X, worldGridChunk.RealChunkBounds.Y, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

                            if (chunkPosition.X == chunkX && chunkPosition.Y == chunkY)
                            {
                                return;
                            }
                        }
                        
                        worldGridChunksLoaded.Enqueue(loadWorldGridChunk(chunkX, chunkY));
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
            ThreadingUtils.assertMainThread();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            int amountChunksLoaded = 0;

            while (worldGridChunksLoaded.IsEmpty == false)
            {
                WorldGridChunk worldGridChunk;

                bool dequeued = worldGridChunksLoaded.TryDequeue(out worldGridChunk);

                if(!dequeued)
                {
                    break;
                }

                int chunkX = (worldGridChunk.RealChunkBounds.X / WorldChunkPixelSize.X);
                int chunkY = (worldGridChunk.RealChunkBounds.Y / WorldChunkPixelSize.Y);
                string chunkKey = chunkX + "," + chunkY;

                if (worldGrid.ContainsKey(chunkKey) == false)
                {
                    worldGrid[chunkKey] = worldGridChunk;
                    worldGrid[chunkKey].OnLoaded(chunkX, chunkY);

                    amountChunksLoaded++;
                }
            }

            stopwatch.Stop();

            if (amountChunksLoaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", amountChunksLoaded + " chunks preloaded took " + stopwatch.ElapsedMilliseconds + "ms");
            }
        }

        public WorldGridChunk GetWorldGridChunk(int chunkX, int chunkY)
        {
            ThreadingUtils.assertMainThread();

            var chunkKey = chunkX + "," + chunkY;

            if(worldGrid.ContainsKey(chunkKey) == false)
            {
                worldGrid[chunkKey] = loadWorldGridChunk(chunkX, chunkY);
                worldGrid[chunkKey].OnLoaded(chunkX, chunkY);
            }

            return worldGrid[chunkKey];
        }

        public void addDurableEntity(DurableEntity durableEntity)
        {
            ThreadingUtils.assertMainThread();

            if (durableEntities.ContainsKey(durableEntity.ID) == false)
                durableEntities[durableEntity.ID] = durableEntity;
        }

        private void garbageCollectWorldGridChunks()
        {
            ThreadingUtils.assertMainThread();
            List<string> deleteList = new List<string>();

            foreach (var chunk in worldGrid)
            {
                var found = false;

                foreach (var durableEntity in durableEntities)
                {
                    if (chunk.Value.RealChunkBounds.Intersects(durableEntity.Value.PreloadedWorldGridChunkPixelBounds))
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
                string[] pos = key.Split(',');

                // Remove containedEntities from interactiveObjects in other neighbor tiles
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j < 1; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        var neighborChunkX = i + Int32.Parse(pos[0]);
                        var neighborChunkY = j + Int32.Parse(pos[1]);
                        var neighborKey = neighborChunkX + "," + neighborChunkY;

                        if (isWorldGridChunkLoaded(neighborChunkX, neighborChunkY))
                        {
                            foreach (var containedEntity in worldGrid[key].ContainedObjects)
                            {
                                worldGrid[neighborKey].OverlappingObjects.Remove(containedEntity);
                            }
                        }
                    }

                foreach (var ambientObject in worldGrid[key].AmbientObjects)
                {
                    ambientObject.Destroy();
                }

                foreach (var containedEntity in worldGrid[key].ContainedObjects)
                {
                    containedEntity.Destroy();
                }

                // Save async
                saveWorldGridChunkAsync(Int32.Parse(pos[0]), Int32.Parse(pos[1]), worldGrid[key]);

                worldGrid.Remove(key);
            }

            GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + deleteList.Count + " world grid chunks");
        }

        public void Update(GameTime gameTime)
        {
            applyLoadedChunks();

            timeSinceLastGarbageCollect += gameTime.ElapsedGameTime;

            if(timeSinceLastGarbageCollect > garbageCollectInterval)
            {
                timeSinceLastGarbageCollect = TimeSpan.Zero;
                garbageCollectWorldGridChunks();
            }

            walkableGrid.Update(gameTime);
            InteriorManager.Update(gameTime);
        }
    }
}