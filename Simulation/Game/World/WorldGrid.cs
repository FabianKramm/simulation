﻿using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Hud;
using Simulation.Game.Generator;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Simulation.Util.Geometry;
using Simulation.Game.Effects;
using System.Linq;
using Simulation.Game.Fractions;
using Simulation.Util.Collision;

namespace Simulation.Game.World
{
    public class WorldGrid: WorldPartManager<ulong, WorldGridChunk>
    {
        public static readonly Point BlockSize = new Point(32, 32);
        public static readonly Point WorldChunkBlockSize = new Point(32, 32); // 32 * 32 BlockSize
        public static readonly Point WorldChunkPixelSize = new Point(WorldChunkBlockSize.X * BlockSize.X, WorldChunkBlockSize.Y * BlockSize.Y);
        public static readonly int RenderOuterBlockRange = 3;

        public WalkableGrid WalkableGrid { get; private set; } = new WalkableGrid();
        public InteriorManager InteriorManager = new InteriorManager();
        public Dictionary<string, LivingEntity> LivingEntities = new Dictionary<string, LivingEntity>();

        public WorldGrid(): base(TimeSpan.FromSeconds(20)) { }

        public void UpdateWorldLink(WorldLink oldWorldLink, WorldLink newWorldLink = null)
        {
            if(oldWorldLink != null)
            {
                try
                {
                    WorldLink oldWorldLinkFrom = GetWorldLinkFromRealPosition(oldWorldLink.ToRealWorldPositionFrom());
                    WorldPart oldWorldPartFrom = (oldWorldLink.FromInteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint(oldWorldLink.FromBlock.X * BlockSize.X, oldWorldLink.FromBlock.Y * BlockSize.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(oldWorldLink.FromInteriorID);
                    oldWorldPartFrom.RemoveWorldLink(oldWorldLinkFrom);
                }
                catch(Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Couldn't remove oldWorldLinkFrom: " + e.Message);
                }

                try
                {
                    WorldLink oldWorldLinkTo = GetWorldLinkFromRealPosition(oldWorldLink.ToRealWorldPositionTo());
                    WorldPart oldWorldPartTo = (oldWorldLink.ToInteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint(oldWorldLink.ToBlock.X * BlockSize.X, oldWorldLink.ToBlock.Y * BlockSize.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(oldWorldLink.ToInteriorID);
                    oldWorldPartTo.RemoveWorldLink(oldWorldLinkTo);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Couldn't remove oldWorldLinkTo: " + e.Message);
                }
            }

            if(newWorldLink != null)
            {
                WorldPart newWorldPartFrom = (newWorldLink.FromInteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint(newWorldLink.FromBlock.X * BlockSize.X, newWorldLink.FromBlock.Y * BlockSize.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(newWorldLink.FromInteriorID);
                WorldPart newWorldPartTo = (newWorldLink.ToInteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint(newWorldLink.ToBlock.X * BlockSize.X, newWorldLink.ToBlock.Y * BlockSize.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(newWorldLink.ToInteriorID);

                newWorldPartFrom.AddWorldLink(newWorldLink);
                newWorldPartTo.AddWorldLink(newWorldLink.SwapFromTo());
            }
        }

        public void SetBlockType(int blockX, int blockY, string InteriorID, int blockType)
        {
            if (InteriorID == Interior.Outside)
            {
                GetFromBlockPoint(blockX, blockY).SetBlockType(blockX, blockY, blockType);
                WalkableGrid.RefreshBlock(blockX, blockY);
            }
            else
            {
                InteriorManager.Get(InteriorID).SetBlockType(blockX, blockY, blockType);
            }
        }

        public int GetBlockType(int blockX, int blockY, string InteriorID)
        {
            if(InteriorID == Interior.Outside)
            {
                return GetFromBlockPoint(blockX, blockY).GetBlockType(blockX, blockY);
            }
            else
            {
                return InteriorManager.Get(InteriorID).GetBlockType(blockX, blockY);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WorldGridChunk GetFromChunkPoint(int chunkX, int chunkY)
        {
            return Get(GeometryUtils.ConvertPointToLong(chunkX, chunkY));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WorldGridChunk GetFromRealPoint(int realX, int realY)
        {
            Point positionChunk = GeometryUtils.GetChunkPosition(realX, realY, WorldChunkPixelSize.X, WorldChunkPixelSize.Y);

            return Get(GeometryUtils.ConvertPointToLong(positionChunk.X, positionChunk.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WorldGridChunk GetFromBlockPoint(int blockX, int blockY)
        {
            Point positionChunk = GeometryUtils.GetChunkPosition(blockX, blockY, WorldChunkBlockSize.X, WorldChunkBlockSize.Y);

            return Get(GeometryUtils.ConvertPointToLong(positionChunk.X, positionChunk.Y));
        }

        public void AddEffectToWorld(Effect effect)
        {
            // Add Effect to chunk
            if (effect.InteriorID == Interior.Outside)
            {
                SimulationGame.World.GetFromRealPoint((int)effect.Position.X, (int)effect.Position.Y).AddEffect(effect);
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(effect.InteriorID).AddEffect(effect);
            }
        }

        public WorldLink GetWorldLinkFromRealPosition(WorldPosition realPosition)
        {
            var worldBlockPosition = realPosition.ToBlockPositionPoint();
            WorldLink worldLink = null;

            if (realPosition.InteriorID == Interior.Outside)
            {
                // Check if we are on a worldLink
                WorldGridChunk worldGridChunk = GetFromRealPoint((int)realPosition.X, (int)realPosition.Y);
                ulong key = GeometryUtils.ConvertPointToLong(worldBlockPosition.X, worldBlockPosition.Y);

                if (worldGridChunk.WorldLinks != null && worldGridChunk.WorldLinks.ContainsKey(key) == true)
                {
                    worldLink = worldGridChunk.WorldLinks[key];
                }
            }
            else
            {
                // Check if we were on a worldLink
                Interior interior = InteriorManager.Get(realPosition.InteriorID);
                ulong key = GeometryUtils.ConvertPointToLong(worldBlockPosition.X, worldBlockPosition.Y);

                if (interior.WorldLinks != null && interior.WorldLinks.ContainsKey(key) == true)
                {
                    worldLink = interior.WorldLinks[key];
                }
            }

            return worldLink;
        }

        protected override WorldGridChunk loadUnguarded(ulong key)
        {
            Point chunkPos = GeometryUtils.GetPointFromLong(key);

            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                GameConsole.WriteLine("ChunkLoading", chunkPos.X + "," + chunkPos.Y + " loaded in main thread");
            }

            var walkableGridChunkPosition = GeometryUtils.GetChunkPosition(chunkPos.X * WorldChunkPixelSize.X, chunkPos.Y * WorldChunkPixelSize.Y, WalkableGrid.WalkableGridPixelChunkSize.X, WalkableGrid.WalkableGridPixelChunkSize.Y);

            WalkableGrid.Get(GeometryUtils.ConvertPointToLong(walkableGridChunkPosition.X, walkableGridChunkPosition.Y));

            return WorldLoader.LoadWorldGridChunk(chunkPos.X, chunkPos.Y);
        }

        private void connectWorldGridChunk(ulong key, WorldGridChunk part)
        {
            ThreadingUtils.assertMainThread();
            Point chunkPos = GeometryUtils.GetPointFromLong(key);
            Point blockPos = GeometryUtils.GetBlockFromReal(part.RealChunkBounds.X, part.RealChunkBounds.Y);

            // Set walkable grid blocking for contained objects
            if (part.ContainedObjects != null)
                foreach (var hitableObject in part.ContainedObjects)
                    hitableObject.ConnectToOverlappingChunks();

            // Set walkable grid blocking for blocks
            for(int i=0;i<WorldChunkBlockSize.X;i++)
                for(int j=0;j<WorldChunkBlockSize.Y;j++)
                {
                    if (CollisionUtils.IsBlockBlocking(part.GetBlockType(blockPos.X + i, blockPos.Y + j)))
                        SimulationGame.World.WalkableGrid.SetBlockNotWalkable(blockPos.X + i, blockPos.Y + j, true);
                }

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    var neighborX = chunkPos.X + i;
                    var neighborY = chunkPos.Y + j;
                    var neighborChunk = Get(GeometryUtils.ConvertPointToLong(neighborX, neighborY), false);

                    if (neighborChunk != null)
                    {
                        // Add neighbor contained objects to self
                        if (neighborChunk.ContainedObjects != null)
                            foreach (var overlappingObject in neighborChunk.ContainedObjects)
                                if (overlappingObject.UnionBounds.Intersects(part.RealChunkBounds))
                                {
                                    part.AddOverlappingObject(overlappingObject);

                                    // Update walkable grid
                                    if (overlappingObject.IsBlocking())
                                    {
                                        SimulationGame.World.WalkableGrid.BlockRect(overlappingObject.BlockingBounds);
                                    }
                                }
                    }
                }
        }

        protected override void saveUnguarded(ulong key, WorldGridChunk part)
        {
            Point chunkPos = GeometryUtils.GetPointFromLong(key);

            WorldLoader.SaveWorldGridChunk(chunkPos.X, chunkPos.Y, part);
        }

        protected override bool shouldRemoveDuringGarbageCollection(ulong key, WorldGridChunk part)
        {
            foreach (var livingEntity in LivingEntities)
            {
                if (livingEntity.Value is DurableEntity && livingEntity.Value.InteriorID == Interior.Outside && part.RealChunkBounds.Intersects(((DurableEntity)livingEntity.Value).PreloadedWorldGridChunkPixelBounds))
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool shouldPersist(ulong key, WorldGridChunk part)
        {
            return part.IsPersistent;
        }

        protected override void unloadPart(ulong key, WorldGridChunk part)
        {
            Point pos = GeometryUtils.GetPointFromLong(key);

            // Remove containedEntities from interactiveObjects in other neighbor tiles
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    var neighborChunkX = i + pos.X;
                    var neighborChunkY = j + pos.Y;
                    var neighborKey = GeometryUtils.ConvertPointToLong(neighborChunkX, neighborChunkY);
                    var neighborPart = Get(neighborKey, false);

                    if (neighborPart != null)
                    {
                        if (part.ContainedObjects != null)
                            foreach (var containedEntity in part.ContainedObjects)
                            {
                                neighborPart.RemoveOverlappingObject(containedEntity);
                            }
                    }
                }

            if (part.AmbientObjects != null)
                foreach (var ambientObject in part.AmbientObjects)
                    ambientObject.Destroy();

            if (part.ContainedObjects != null)
                foreach (var containedEntity in part.ContainedObjects)
                    containedEntity.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ICollection<ulong> keys = GetKeys();

            for (int i = 0; i < keys.Count; i++)
            {
                var worldGridChunkItem = Get(keys.ElementAt(i), false);

                if (worldGridChunkItem != null)
                {
                    if (!worldGridChunkItem.Connected)
                    {
                        connectWorldGridChunk(keys.ElementAt(i), worldGridChunkItem);
                        worldGridChunkItem.Connected = true;
                    }

                    worldGridChunkItem.Update(gameTime);
                }
            }

            WalkableGrid.Update(gameTime);
            InteriorManager.Update(gameTime);

            if (SimulationGame.IsGodMode)
            {
                SimulationGame.Player.Update(gameTime);
            }
        }
    }
}
