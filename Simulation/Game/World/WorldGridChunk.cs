using Simulation.Game.Objects;
using Simulation.Util.Geometry;
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
    }
}
