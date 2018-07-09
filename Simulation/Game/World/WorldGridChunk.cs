using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.Base;
using Simulation.Util;
using System.Collections.Generic;

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
        private BlockType[,] blockingGrid;

        public Rectangle realChunkBounds;

        // These objects are just passing by or are overlapping with this chunk
        public List<HitableObject> OverlappingObjects;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> ContainedObjects;

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        // These objects link to the interiors
        public List<WorldLink> worldLinks;

        private WorldGridChunk() { }

        public WorldGridChunk(int realX, int realY)
        {
            blockingGrid = new BlockType[WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y];
            realChunkBounds = new Rectangle(realX, realY, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
        }

        public Point getChunkPosition()
        {
            return GeometryUtils.getChunkPosition(realChunkBounds.X, realChunkBounds.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
        }

        public BlockType getBlockType(int blockX, int blockY)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public void setBlockType(int blockX, int blockY, BlockType blockType)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public void AddWorldLink(WorldLink worldLink)
        {
            if (worldLinks == null)
                worldLinks = new List<WorldLink>();

            if (worldLinks.Contains(worldLink) == false)
                worldLinks.Add(worldLink);
        }

        public void RemoveWorldLink(WorldLink worldLink)
        {
            if (worldLinks != null)
            {
                worldLinks.Remove(worldLink);

                if (worldLinks.Count == 0)
                {
                    worldLinks = null;
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
                foreach (DrawableObject drawableObject in ContainedObjects)
                    if (drawableObject is HitableObject)
                        SimulationGame.World.walkableGrid.addInteractiveObject((HitableObject)drawableObject);

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int neighborX = chunkX + i;
                    int neighborY = chunkY + j;
                    WorldGridChunk worldGridChunk = SimulationGame.World.getWorldGridChunk(neighborX, neighborY);

                    if (SimulationGame.World.isWorldGridChunkLoaded(neighborX, neighborY) == true)
                    {
                        // Add own contained objects to neighbor
                        if(ContainedObjects != null)
                            foreach (HitableObject containedObject in ContainedObjects)
                                if (containedObject.unionBounds.Intersects(worldGridChunk.realChunkBounds))
                                    worldGridChunk.AddOverlappingObject(containedObject);

                        // Add neighbor contained objects to self
                        if(worldGridChunk.ContainedObjects != null)
                            foreach (HitableObject overlappingObject in worldGridChunk.ContainedObjects)
                                if (overlappingObject.unionBounds.Intersects(realChunkBounds))
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
