using Simulation.Game.Base;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public enum BlockType
    {
        GRASS_01 = 0,
        GRASS_02,
        GRASS_03,
        GRASS_04,
        GRASS_WATERHOLE,
    }

    public class WorldGridChunk
    {
        private BlockType[,] blockingGrid;

        public List<HitableObject> interactiveObjects;
        public List<DrawableObject> ambientObjects;

        public WorldGridChunk()
        {
            blockingGrid = new BlockType[World.WorldChunkSize.X, World.WorldChunkSize.Y];
        }

        public BlockType getBlockType(int x, int y)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(x, y, World.WorldChunkSize.X, World.WorldChunkSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public void setBlockType(int x, int y, BlockType blockType)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(x, y, World.WorldChunkSize.X, World.WorldChunkSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public void addHitableObject(HitableObject hitableObject)
        {
            if (interactiveObjects == null)
                interactiveObjects = new List<HitableObject>();

            interactiveObjects.Add(hitableObject);
        }

        public void removeHitableObject(HitableObject hitableObject)
        {
            if (interactiveObjects != null)
            {
                interactiveObjects.Remove(hitableObject);

                if (interactiveObjects.Count == 0)
                {
                    interactiveObjects = null;
                }
            }
        }

        public void addAmbientObject(DrawableObject ambientObject)
        {
            if (ambientObjects == null)
                ambientObjects = new List<DrawableObject>();

            ambientObjects.Add(ambientObject);
        }

        public void removeAmbientObject(DrawableObject ambientObject)
        {
            if (ambientObjects != null)
            {
                ambientObjects.Remove(ambientObject);

                if (ambientObjects.Count == 0)
                {
                    ambientObjects = null;
                }
            }
        }
    }
}
