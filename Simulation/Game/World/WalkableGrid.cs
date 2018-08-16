using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Generator;
using Simulation.Util;
using System;
using Simulation.Util.Geometry;
using Simulation.Game.Fractions;
using Simulation.Util.Collision;
using Simulation.Game.Objects.Entities;

/*
 * The WalkableGrid is only used for quick pathfinding
 */
namespace Simulation.Game.World
{
    // TODO: Cleanup and save unused chunks
    public class WalkableGrid: WorldPartManager<ulong, WalkableGridChunk>
    {
        public static Point WalkableGridBlockChunkSize = WorldGrid.WorldChunkBlockSize;
        public static Point WalkableGridPixelChunkSize = new Point(WalkableGridBlockChunkSize.X * WorldGrid.BlockSize.X, WalkableGridBlockChunkSize.Y * WorldGrid.BlockSize.Y);

        public static int WalkableGridArrayChunkCount = WalkableGridBlockChunkSize.X * WalkableGridBlockChunkSize.Y / 32;

        public WalkableGrid(): base(TimeSpan.FromSeconds(30)) { }

        protected override WalkableGridChunk loadUnguarded(ulong key)
        {
            Point point = GeometryUtils.GetPointFromLong(key);

            return WorldLoader.LoadWalkableGridChunk(point.X, point.Y);
        }

        protected override void saveUnguarded(ulong key, WalkableGridChunk part)
        {
            Point point = GeometryUtils.GetPointFromLong(key);

            WorldLoader.SaveWalkableGridChunk(point.X, point.Y, part);
        }

        public bool IsPositionWalkable(int realX, int realY)
        {
            var blockPosition = GeometryUtils.GetChunkPosition(realX, realY, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            return IsBlockWalkable(blockPosition.X, blockPosition.Y);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var chunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var chunk = Get(GeometryUtils.ConvertPointToLong(chunkPosition.X, chunkPosition.Y));

            return chunk.IsBlockWalkable(blockX, blockY);
        }

        private void setBlockNotWalkable(int blockX, int blockY, bool notWalkable)
        {
            var chunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var chunk = Get(GeometryUtils.ConvertPointToLong(chunkPosition.X, chunkPosition.Y), true);

            chunk.SetWalkable(blockX, blockY, notWalkable);
        }

        public void BlockRect(Rect blockingBounds)
        {
            Point blockTopLeft = GeometryUtils.GetChunkPosition(blockingBounds.Left, blockingBounds.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point blockBottomRight = GeometryUtils.GetChunkPosition(blockingBounds.Right, blockingBounds.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            for (int blockX = blockTopLeft.X; blockX <= blockBottomRight.X; blockX++)
                for (int blockY = blockTopLeft.Y; blockY <= blockBottomRight.Y; blockY++)
                {
                    Point walkableGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

                    if (IsLoaded(GeometryUtils.ConvertPointToLong(walkableGridChunkPos.X, walkableGridChunkPos.Y)))
                    {
                        setBlockNotWalkable(blockX, blockY, true);
                    }
                }
        }

        public void RefreshBlock(int blockX, int blockY)
        {
            Point walkableGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            if (IsLoaded(GeometryUtils.ConvertPointToLong(walkableGridChunkPos.X, walkableGridChunkPos.Y)))
            {
                Point worldGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(worldGridChunkPos.X, worldGridChunkPos.Y);
                int blockType = worldGridChunk.GetBlockType(blockX, blockY);

                if (CollisionUtils.IsBlockBlocking(blockType))
                {
                    setBlockNotWalkable(blockX, blockY, true);
                    return;
                }
                
                var found = false;

                if (worldGridChunk.ContainedObjects != null)
                    foreach (HitableObject interactiveObject in worldGridChunk.ContainedObjects)
                        if (interactiveObject.IsBlocking() && interactiveObject.BlockingBounds.Intersects(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
                        {
                            found = true;
                            break;
                        }

                if (worldGridChunk.OverlappingObjects != null)
                    foreach (HitableObject interactiveObject in worldGridChunk.OverlappingObjects)
                        if (interactiveObject.IsBlocking() && interactiveObject.BlockingBounds.Intersects(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
                        {
                            found = true;
                            break;
                        }

                setBlockNotWalkable(blockX, blockY, found);
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

                    if (IsLoaded(GeometryUtils.ConvertPointToLong(walkableGridChunkPos.X, walkableGridChunkPos.Y)))
                    {
                        Point worldGridChunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(worldGridChunkPos.X, worldGridChunkPos.Y);
                        int blockType = worldGridChunk.GetBlockType(blockX, blockY);

                        if (CollisionUtils.IsBlockBlocking(blockType))
                            continue;

                        var found = false;

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (HitableObject interactiveObject in worldGridChunk.ContainedObjects)
                                if (interactiveObject.IsBlocking() && interactiveObject.BlockingBounds.Intersects(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
                                {
                                    found = true;
                                    break;
                                }

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject interactiveObject in worldGridChunk.OverlappingObjects)
                                if (interactiveObject.IsBlocking() && interactiveObject.BlockingBounds.Intersects(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
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

        protected override bool shouldPersist(ulong key, WalkableGridChunk part)
        {
            return part.IsPersistent;
        }

        protected override bool shouldRemoveDuringGarbageCollection(ulong key, WalkableGridChunk part)
        {
            foreach (var livingEntity in SimulationGame.World.LivingEntities)
            {
                if (livingEntity.Value is DurableEntity && livingEntity.Value.InteriorID == Interior.Outside && part.realChunkBounds.Intersects(((DurableEntity)livingEntity.Value).PreloadedWorldGridChunkPixelBounds))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void unloadPart(ulong key, WalkableGridChunk part)
        {
            // Noop
        }
    }
}
