using Microsoft.Xna.Framework;
using Simulation.Game.MetaData.World;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.Generator
{
    public class WorldSegmentPlanner
    {
        public static readonly float PerlinRange = 1.5f;

        public static readonly Point WorldSegmentChunkSize = new Point(4, 4); // 4 * 4 WorldGridChunks
        public static readonly Point WorldSegmentBlockSize = new Point(WorldSegmentChunkSize.X * WorldGrid.WorldChunkBlockSize.X, WorldSegmentChunkSize.Y * WorldGrid.WorldChunkBlockSize.Y);

        private Point startBlockPosition;
        private Point startChunkPosition;
        
        private FastNoise noiseGenerator;
        private BiomeType biome;
        private BlockInformation[,] blockInformation;

        public WorldSegmentPlanner(int seed, Point blockPosition, BiomeType biomeType)
        {
            noiseGenerator = new FastNoise(seed);
            blockInformation = new BlockInformation[WorldSegmentBlockSize.X, WorldSegmentBlockSize.Y];
            startChunkPosition = GeometryUtils.GetChunkPosition(blockPosition.X, blockPosition.Y, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
            startBlockPosition = blockPosition;
            biome = biomeType;
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
                                blockInformation[blockPositionWithin.X, blockPositionWithin.Y] = null;
                            }
                            else
                            {
                                doesSegmentAlreadyExist = false;

                                blockInformation[blockPositionWithin.X, blockPositionWithin.Y] = new BlockInformation()
                                {
                                    BlockId = biome.MainTilesIds[0]
                                };
                            }
                        }
                }

            return !doesSegmentAlreadyExist;
        }

        public void Generate()
        {
            planHeightMap();
        }

        private void reservePoISpace()
        {
            
        }

        private void planHeightMap()
        {
            /*if (biome.DoubleNegativeElevationProbability == 0 && biome.NegativeElevationProbability == 0 && biome.ElevationProbability == 0 && biome.DoubleElevationProbability == 0)
                return;

            
            
            for (int x = 0; x < WorldSegmentBlockSize.X; x++)
                for (int y = 0; y < WorldSegmentBlockSize.Y; y++)
                {
                    if (blockInformation[x, y] == null)
                        continue;

                    float avg = 0.0f;

                    for (int i = 0; x < PerlinInterpolation.X; x++)
                        for (int j = 0; y < PerlinInterpolation.Y; y++)
                            avg += noiseGenerator.GetPerlin(x * PerlinInterpolation.X + i, j * PerlinInterpolation.Y + y);

                    var noise = Math.Round(avg / interpolationSize, 2);

                    if (noise < doubleNegativeElevationTreshold)
                    {
                        blockInformation[x, y].BlockId = biome.DoubleNegativeElevationTileIds[0].ElevatedCenter;
                        blockInformation[x, y].IsDoubleNegativeElevated = true;
                    }
                    else if(noise < negativeElevationTreshold)
                    {
                        blockInformation[x, y].BlockId = biome.NegativeElevationTileIds[0].ElevatedCenter;
                        blockInformation[x, y].IsNegativeElevated = true;
                    }
                    else if(noise < noElevationTreshold)
                    {
                        blockInformation[x, y].IsNotElevated = true;
                    }
                    else if (noise < elevationTreshold)
                    {
                        blockInformation[x, y].BlockId = biome.ElevationTileIds[0].ElevatedCenter;
                        blockInformation[x, y].IsElevated = true;
                    }
                    else if (noise < doubleElevationTreshold)
                    {
                        blockInformation[x, y].BlockId = biome.DoubleElevationTileIds[0].ElevatedCenter;
                        blockInformation[x, y].IsDoubleElevated = true;
                    }
                    else
                    {
                        blockInformation[x, y].IsNotElevated = true;
                    }
                }*/
        }

        private void interpolateHeightMap()
        {
            for (int y = WorldSegmentBlockSize.Y - 1; y >= 0; y--)
                for (int x = WorldSegmentBlockSize.X - 1; x >= 0; x--)
                {
                    

                }
        }

        public bool IsRectFree(Rect bounds)
        {
            int right = bounds.X + bounds.Width;
            int bottom = bounds.Y + bounds.Height;

            for (int x = bounds.X; x < right; x++)
                for (int y = bounds.Y; y < bottom; y++)
                    if (blockInformation[x, y].IsReserved)
                        return false;

            return true;
        }

        // Create WorldGridChunks
        public void Build()
        {
            for (int i = 0; i < WorldSegmentChunkSize.X; i++)
                for (int j = 0; j < WorldSegmentChunkSize.Y; j++)
                {
                    var worldGridChunkExists = WorldLoader.DoesWorldGridChunkExist(startChunkPosition.X + i, startChunkPosition.Y + j);

                    if (worldGridChunkExists == true)
                        continue;
                    
                    if (blockInformation[i * WorldGrid.WorldChunkBlockSize.X, j * WorldGrid.WorldChunkBlockSize.Y] == null)
                        continue;

                    var chunkBlockPosition = new Point(startBlockPosition.X + i * WorldGrid.WorldChunkBlockSize.X, startBlockPosition.Y + j * WorldGrid.WorldChunkBlockSize.Y);

                    WorldGridChunk worldGridChunk = new WorldGridChunk(WorldGrid.BlockSize.X * chunkBlockPosition.X, WorldGrid.BlockSize.Y * chunkBlockPosition.Y);
                    WalkableGridChunk walkableGridChunk = WalkableGridChunk.CreateEmpty(startChunkPosition.X + i, startChunkPosition.Y + j);

                    worldGridChunk.SetBiomeType(biome);

                    for (int x = 0; x < WorldGrid.WorldChunkBlockSize.X; x++)
                        for (int y = 0; y < WorldGrid.WorldChunkBlockSize.Y; y++)
                        {
                            var blockPosition = new Point(chunkBlockPosition.X + x, chunkBlockPosition.Y + y);
                            var worldSegmentPosition = new Point(i * WorldGrid.WorldChunkBlockSize.X + x, j * WorldGrid.WorldChunkBlockSize.Y + y);

                            worldGridChunk.SetBlockType(blockPosition.X, blockPosition.Y, blockInformation[worldSegmentPosition.X, worldSegmentPosition.Y].BlockId);

                            if(CollisionUtils.GetBlockingTypeFromBlock(blockInformation[worldSegmentPosition.X, worldSegmentPosition.Y].BlockId) == Enums.BlockingType.BLOCKING)
                                walkableGridChunk.SetWalkable(blockPosition.X, blockPosition.Y, false);
                        }

                    WorldLoader.SaveWorldGridChunk(startChunkPosition.X + i, startChunkPosition.Y + j, worldGridChunk);
                    WorldLoader.SaveWalkableGridChunk(startChunkPosition.X + i, startChunkPosition.Y + j, walkableGridChunk);
                }
        }
    }
}
