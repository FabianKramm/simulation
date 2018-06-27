using Microsoft.Xna.Framework;
using Newtonsoft.Json;
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
        [JsonProperty]
        private BlockType[,] blockingGrid;

        public Rectangle realChunkBounds;

        // These objects are just passing by or are overlapping with this chunk
        [JsonIgnore]
        public List<HitableObject> interactiveObjects;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> containedObjects;

        // These objects are not important for the world and just displayed here
        public List<AmbientObject> ambientObjects;

        public WorldGridChunk(int realX, int realY)
        {
            blockingGrid = new BlockType[World.WorldChunkBlockSize.X, World.WorldChunkBlockSize.Y];
            realChunkBounds = new Rectangle(realX, realY, World.WorldChunkPixelSize.X, World.WorldChunkPixelSize.Y);
        }

        public Point getChunkPosition()
        {
            return GeometryUtils.getChunkPosition(realChunkBounds.X, realChunkBounds.Y, World.WorldChunkPixelSize.X, World.WorldChunkPixelSize.Y);
        }

        public BlockType getBlockType(int blockX, int blockY)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(blockX, blockY, World.WorldChunkBlockSize.X, World.WorldChunkBlockSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public void setBlockType(int blockX, int blockY, BlockType blockType)
        {
            var projectedPosition = GeometryUtils.getPositionWithinChunk(blockX, blockY, World.WorldChunkBlockSize.X, World.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public void addContainedObject(HitableObject interactiveObject)
        {
            if (containedObjects == null)
                containedObjects = new List<HitableObject>();

            containedObjects.Add(interactiveObject);
        }

        public void removeContainedObject(HitableObject interactiveObject)
        {
            if (containedObjects != null)
            {
                containedObjects.Remove(interactiveObject);

                if (containedObjects.Count == 0)
                {
                    containedObjects = null;
                }
            }
        }

        public void addInteractiveObject(HitableObject hitableObject)
        {
            if (interactiveObjects == null)
                interactiveObjects = new List<HitableObject>();

            interactiveObjects.Add(hitableObject);
        }

        public void removeInteractiveObject(HitableObject hitableObject)
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

        public void addAmbientObject(AmbientObject ambientObject)
        {
            if (ambientObjects == null)
                ambientObjects = new List<AmbientObject>();

            ambientObjects.Add(ambientObject);
        }

        public void removeAmbientObject(AmbientObject ambientObject)
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
