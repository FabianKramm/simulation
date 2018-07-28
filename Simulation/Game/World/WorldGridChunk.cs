using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Util.Geometry;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Game.World
{
    public class WorldGridChunk: WorldPart
    {
        public Rect RealChunkBounds;

        // These objects are just passing by or are overlapping with this chunk
        public List<HitableObject> OverlappingObjects;

        private WorldGridChunk() { }

        public WorldGridChunk(int realX, int realY)
        {
            Dimensions = WorldGrid.WorldChunkBlockSize;
            blockingGrid = new int[WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y];
            RealChunkBounds = new Rect(realX, realY, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
        }

        public override int GetBlockType(int blockX, int blockY)
        {
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public override void SetBlockType(int blockX, int blockY, int blockType)
        {
            Debug.Assert(Connected == false, "Cannot set block type, when already connected to world!");

            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public void SetPersistent(bool persistent)
        {
            IsPersistent = persistent;

            Point chunkPosition = GeometryUtils.GetChunkPosition(RealChunkBounds.X, RealChunkBounds.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

            SimulationGame.World.WalkableGrid.Get(GeometryUtils.ConvertPointToLong(chunkPosition.X, chunkPosition.Y)).SetPersistent(persistent);
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
