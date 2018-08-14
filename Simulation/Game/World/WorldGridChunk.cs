using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.MetaData.World;
using Simulation.Game.Objects;
using Simulation.Game.Serialization;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public class WorldGridChunk: WorldPart
    {
        [Serialize]
        public Rect RealChunkBounds;

        [Serialize]
        private int biomeId = -1;

        public BiomeType BiomeType
        {
            get => BiomeType.lookup.ContainsKey(biomeId) ? BiomeType.lookup[biomeId] : null;
        }

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
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

       

        public void SetBiomeType(BiomeType biomeType)
        {
            biomeId = biomeType.ID;
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

        public override void SetPersistent(bool persistent)
        {
            base.SetPersistent(persistent);

            Point chunkPosition = GeometryUtils.GetChunkPosition(RealChunkBounds.X, RealChunkBounds.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            SimulationGame.World.WalkableGrid.Get(GeometryUtils.ConvertPointToLong(chunkPosition.X, chunkPosition.Y)).SetPersistent(persistent);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(SimulationGame.Player.InteriorID == Interior.Outside && SimulationGame.VisibleArea.Intersects(RealChunkBounds))
            {
                // Update Ambient Objects
                for (int i = 0; AmbientObjects != null && i < AmbientObjects.Count; i++) // Avoid collection changed problem with updatePosition and disconnectWorld
                    AmbientObjects[i].Update(gameTime);
            }
        }
    }
}
