using Microsoft.Xna.Framework;
using Simulation.Game.MetaData.World;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Generator
{
    public class WorldSegmentPlanner
    {
        public static readonly float PerlinRange = 1.5f;

        public static readonly Point WorldSegmentChunkSize = new Point(4, 4); // 4 * 4 WorldGridChunks
        public static readonly Point WorldSegmentBlockSize = new Point(WorldSegmentChunkSize.X * WorldGrid.WorldChunkBlockSize.X, WorldSegmentChunkSize.Y * WorldGrid.WorldChunkBlockSize.Y);

        private Point startBlockPosition;
        private Point startChunkPosition;
        
        public FastNoise NoiseGenerator;
        public Random Random;

        public BiomeType Biome;
        public BlockInformation[,] Blocks;

        public WorldSegmentPlanner(int seed, Point blockPosition, BiomeType biomeType)
        {
            Random = new Random(seed);
            NoiseGenerator = new FastNoise(seed);
            Blocks = new BlockInformation[WorldSegmentBlockSize.X, WorldSegmentBlockSize.Y];
            startChunkPosition = GeometryUtils.GetChunkPosition(blockPosition.X, blockPosition.Y, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
            startBlockPosition = blockPosition;
            Biome = biomeType;
        }

        public bool Init()
        {
            bool doesSegmentAlreadyExist = true;

            for (int i = 0; i < WorldSegmentChunkSize.X; i++)
                for (int j = 0; j < WorldSegmentChunkSize.Y; j++)
                {
                    var worldGridChunkExists = WorldLoader.DoesWorldGridChunkExist(startChunkPosition.X + i, startChunkPosition.Y + j);
                    var chunkBlockPosition = new Point(i * WorldGrid.WorldChunkBlockSize.X, j * WorldGrid.WorldChunkBlockSize.Y);

                    for (int x = 0; x < WorldGrid.WorldChunkBlockSize.X; x++)
                        for (int y = 0; y < WorldGrid.WorldChunkBlockSize.Y; y++)
                        {
                            var blockPositionWithin = new Point(chunkBlockPosition.X + x, chunkBlockPosition.Y + y);

                            if (worldGridChunkExists)
                            {
                                Blocks[blockPositionWithin.X, blockPositionWithin.Y] = null;
                            }
                            else
                            {
                                doesSegmentAlreadyExist = false;

                                Blocks[blockPositionWithin.X, blockPositionWithin.Y] = new BlockInformation()
                                {
                                    BlockId = Biome.MainTilesIds[0]
                                };
                            }
                        }
                }

            return !doesSegmentAlreadyExist;
        }

        public void Generate()
        {
            ElevationGenerator.GenerateHeightMap(this);
        }

        private void reservePoISpace()
        {
            
        }

        public bool BlockExists(int blockX, int blockY)
        {
            if (blockX < 0 || blockX >= WorldSegmentBlockSize.X || blockY < 0 || blockY >= WorldSegmentBlockSize.Y || Blocks[blockX, blockY] == null)
            {
                return false;
            }

            return true;
        }
         
        // Return array indices looks like:
        // 0 3 5
        // 1 x 6
        // 2 4 7
        public BlockInformation[] GetNeighbors(int blockX, int blockY)
        {
            var neighbors = new List<BlockInformation>();

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int neighborX = blockX + i;
                    int neighborY = blockY + j;

                    if (neighborX < 0 || neighborX >= WorldSegmentBlockSize.X || neighborY < 0 || neighborY >= WorldSegmentBlockSize.Y || Blocks[neighborX, neighborY] == null)
                    {
                        neighbors.Add(BlockInformation.None);
                        continue;
                    }

                    neighbors.Add(Blocks[neighborX, neighborY]);
                }

            return neighbors.ToArray();
        }

        public bool IsRectFree(Rect bounds)
        {
            int right = bounds.X + bounds.Width;
            int bottom = bounds.Y + bounds.Height;

            for (int x = bounds.X; x < right; x++)
                for (int y = bounds.Y; y < bottom; y++)
                    if (Blocks[x, y].IsReserved)
                        return false;

            return true;
        }

        // Create WorldGridChunks
        public void Build()
        {
            //ElevationGenerator.PrintHeightMap(this);

            for (int i = 0; i < WorldSegmentChunkSize.X; i++)
                for (int j = 0; j < WorldSegmentChunkSize.Y; j++)
                {
                    var worldGridChunkExists = WorldLoader.DoesWorldGridChunkExist(startChunkPosition.X + i, startChunkPosition.Y + j);

                    if (worldGridChunkExists == true)
                        continue;
                    
                    if (Blocks[i * WorldGrid.WorldChunkBlockSize.X, j * WorldGrid.WorldChunkBlockSize.Y].BlockId == BlockType.Invalid)
                        continue;

                    var chunkBlockPosition = new Point(startBlockPosition.X + i * WorldGrid.WorldChunkBlockSize.X, startBlockPosition.Y + j * WorldGrid.WorldChunkBlockSize.Y);

                    WorldGridChunk worldGridChunk = new WorldGridChunk(WorldGrid.BlockSize.X * chunkBlockPosition.X, WorldGrid.BlockSize.Y * chunkBlockPosition.Y);
                    WalkableGridChunk walkableGridChunk = WalkableGridChunk.CreateEmpty(startChunkPosition.X + i, startChunkPosition.Y + j);

                    worldGridChunk.SetBiomeType(Biome);

                    for (int x = 0; x < WorldGrid.WorldChunkBlockSize.X; x++)
                        for (int y = 0; y < WorldGrid.WorldChunkBlockSize.Y; y++)
                        {
                            var blockPosition = new Point(chunkBlockPosition.X + x, chunkBlockPosition.Y + y);
                            var worldSegmentPosition = new Point(i * WorldGrid.WorldChunkBlockSize.X + x, j * WorldGrid.WorldChunkBlockSize.Y + y);

                            worldGridChunk.SetBlockType(blockPosition.X, blockPosition.Y, Blocks[worldSegmentPosition.X, worldSegmentPosition.Y].BlockId);

                            if(CollisionUtils.IsBlockBlocking(Blocks[worldSegmentPosition.X, worldSegmentPosition.Y].BlockId))
                                walkableGridChunk.SetWalkable(blockPosition.X, blockPosition.Y, false);
                        }

                    WorldLoader.SaveWorldGridChunk(startChunkPosition.X + i, startChunkPosition.Y + j, worldGridChunk);
                    WorldLoader.SaveWalkableGridChunk(startChunkPosition.X + i, startChunkPosition.Y + j, walkableGridChunk);
                }
        }
    }
}
