using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Objects;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Simulation.Game.World
{
    public enum BlockType
    {
        NONE = 0,
        GRASS_01,
        GRASS_02,
        GRASS_03,
        GRASS_04,
        GRASS_WATERHOLE,
    }

    public class WorldGridChunk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WorldGridChunk GetWorldGridChunk(int realX, int realY)
        {
            Point positionChunk = GeometryUtils.GetChunkPosition(realX, realY, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

            return SimulationGame.World.GetWorldGridChunk(positionChunk.X, positionChunk.Y);
        }

        private BlockType[,] blockingGrid;

        public Rect RealChunkBounds;

        // These objects are just passing by or are overlapping with this chunk
        public List<HitableObject> OverlappingObjects;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> ContainedObjects;

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        // These objects link to the interiors
        public Dictionary<string, WorldLink> WorldLinks;

        private WorldGridChunk() { }

        public WorldGridChunk(int realX, int realY)
        {
            blockingGrid = new BlockType[WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y];
            RealChunkBounds = new Rect(realX, realY, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
        }

        public Point getChunkPosition()
        {
            return GeometryUtils.GetChunkPosition(RealChunkBounds.X, RealChunkBounds.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
        }

        public BlockType GetBlockType(int blockX, int blockY)
        {
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public void SetBlockType(int blockX, int blockY, BlockType blockType)
        {
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
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
            if (WorldLinks != null)
            {
                WorldLinks.Remove(worldLink.FromBlock.X + "," + worldLink.FromBlock.Y);

                if (WorldLinks.Count == 0)
                {
                    WorldLinks = null;
                }
            }
        }

        public void AddContainedObject(HitableObject containedObject)
        {
            if (ContainedObjects == null)
                ContainedObjects = new List<HitableObject>();

            if (ContainedObjects.Contains(containedObject) == false)
                ContainedObjects.Add(containedObject);
        }

        public void RemoveContainedObject(HitableObject containedObject)
        {
            if (ContainedObjects != null)
            {
                ContainedObjects.Remove(containedObject);

                if (ContainedObjects.Count == 0)
                {
                    ContainedObjects = null;
                }
            }
        }

        public void AddOverlappingObject(HitableObject overlappingObject)
        {
            if (OverlappingObjects == null)
                OverlappingObjects = new List<HitableObject>();

            if (OverlappingObjects.Contains(overlappingObject) == false)
                OverlappingObjects.Add(overlappingObject);
        }

        public void RemoveOverlappingObject(HitableObject overlappingObject)
        {
            if (OverlappingObjects != null)
            {
                OverlappingObjects.Remove(overlappingObject);

                if (OverlappingObjects.Count == 0)
                {
                    OverlappingObjects = null;
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

        /*
            This function is executed when a new chunk was loaded and objects from other chunks could overlap with this chunk
         */
        public void OnLoaded(int chunkX, int chunkY)
        {
            ThreadingUtils.assertMainThread();

            // Set walkable grid blocking for contained objects
            if (ContainedObjects != null)
                foreach (GameObject drawableObject in ContainedObjects)
                    if (drawableObject is HitableObject)
                        SimulationGame.World.walkableGrid.addInteractiveObject((HitableObject)drawableObject);

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int neighborX = chunkX + i;
                    int neighborY = chunkY + j;
                    
                    if (SimulationGame.World.isWorldGridChunkLoaded(neighborX, neighborY) == true)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunk(neighborX, neighborY);

                        // Add own contained objects to neighbor
                        if (ContainedObjects != null)
                            foreach (HitableObject containedObject in ContainedObjects)
                                if (containedObject.UnionBounds.Intersects(worldGridChunk.RealChunkBounds))
                                    worldGridChunk.AddOverlappingObject(containedObject);

                        // Add neighbor contained objects to self
                        if(worldGridChunk.ContainedObjects != null)
                            foreach (HitableObject overlappingObject in worldGridChunk.ContainedObjects)
                                if (overlappingObject.UnionBounds.Intersects(RealChunkBounds))
                                {
                                    AddOverlappingObject(overlappingObject);

                                    // Update walkable grid
                                    SimulationGame.World.walkableGrid.addInteractiveObject(overlappingObject);
                                }
                    }
                }
        }
    }
}
