using Simulation.Game.Objects;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;
using System.Linq;

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

    public class WorldGridChunk: WorldPart
    {
        public Rect RealChunkBounds;

        // These objects are just passing by or are overlapping with this chunk
        public List<HitableObject> OverlappingObjects;

        private WorldGridChunk() { }

        public WorldGridChunk(int realX, int realY)
        {
            blockingGrid = new BlockType[WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y];
            RealChunkBounds = new Rect(realX, realY, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
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

        /*
            This function is executed when a new chunk was loaded and objects from other chunks could overlap with this chunk
         */
        public void OnLoaded(int chunkX, int chunkY)
        {
            ThreadingUtils.assertMainThread();

            // Set walkable grid blocking for contained objects
            if (ContainedObjects != null)
                foreach (var hitableObject in ContainedObjects)
                    hitableObject.ConnectToWorld();

            // Set walkable grid blocking for contained objects
            if (WorldLinks != null)
                foreach (var worldLink in WorldLinks)
                    SimulationGame.World.walkableGrid.setBlockNotWalkable(worldLink.Value.FromBlock.X, worldLink.Value.FromBlock.Y, true);

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int neighborX = chunkX + i;
                    int neighborY = chunkY + j;
                    
                    if (SimulationGame.World.isWorldGridChunkLoaded(neighborX, neighborY) == true)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunk(neighborX, neighborY);

                        // Add neighbor contained objects to self
                        if(worldGridChunk.ContainedObjects != null)
                            foreach (var overlappingObject in worldGridChunk.ContainedObjects)
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
