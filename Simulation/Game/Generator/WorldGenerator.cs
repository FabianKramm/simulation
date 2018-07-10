using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Generator.Factories;
using Simulation.Game.Generator.InteriorGeneration;
using Simulation.Game.Hud;
using Simulation.Game.World;
using Simulation.Util;
using System;
using System.Collections.Generic;
using Simulation.Util.Geometry;

namespace Simulation.Game.Generator
{
    public class WorldGenerator
    {
        private static Point generatedChunkBlockSize = new Point(128, 128);

        public Random random;
        private object generatorLock = new object();

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public void generateChunk(int blockX, int blockY)
        {
            lock(generatorLock)
            {
                generateWorld(blockX, blockY);
            }
        }

        public static void ResetWorld()
        {
            Util.Util.clearWorldFiles();
        }

        private void generateWorld(int blockX, int blockY)
        {
            Point chunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, generatedChunkBlockSize.X, generatedChunkBlockSize.Y);
            
            var newX = chunkPosition.X * generatedChunkBlockSize.X;
            var newY = chunkPosition.Y * generatedChunkBlockSize.Y;

            Point worldGridChunkPosition = GeometryUtils.GetChunkPosition(newX, newY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            if(WorldLoader.DoesWorldGridChunkExist(worldGridChunkPosition.X, worldGridChunkPosition.Y))
            {
                return;
            }

            Dictionary<(int, int), WorldGridChunk> worldGrid = new Dictionary<(int, int), WorldGridChunk>();
            Dictionary<(int, int), WalkableGridChunk> walkableGrid = new Dictionary<(int, int), WalkableGridChunk>();

            GameConsole.WriteLine("WorldGeneration", "Generate new Chunk at " + chunkPosition.X + "," + chunkPosition.Y);

            // Loop over Blocks
            for (int i = newX; i < (newX + generatedChunkBlockSize.X); i++)
                for (int j = newY; j < (newY + generatedChunkBlockSize.Y); j++)
                {
                    Point worldGridChunk = GeometryUtils.GetChunkPosition(i, j, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                    Point walkableGridChunk = GeometryUtils.GetChunkPosition(i, j, WalkableGrid.WalkableGridBlockChunkSize.X, WalkableGrid.WalkableGridBlockChunkSize.Y);

                    if(worldGrid.ContainsKey((worldGridChunk.X, worldGridChunk.Y)) == false)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)] = new WorldGridChunk(worldGridChunk.X * WorldGrid.WorldChunkPixelSize.X, worldGridChunk.Y * WorldGrid.WorldChunkPixelSize.Y);
                    }

                    if (walkableGrid.ContainsKey((walkableGridChunk.X, walkableGridChunk.Y)) == false)
                    {
                        walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)] = WalkableGridChunk.createEmpty(walkableGridChunk.X, walkableGridChunk.Y);
                    }
                    
                    int Value = random.Next(0, 300);

                    if (Value <= 2)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].SetBlockType(i, j, BlockType.GRASS_WATERHOLE);

                        WalkableGrid.setBlockNotWalkableInChunk(walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)], i, j, true);
                    }
                    else
                    {
                        int randomTexture = random.Next((int)BlockType.GRASS_01, (int)BlockType.GRASS_04 + 1);

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].SetBlockType(i, j, (BlockType)randomTexture);

                        if(Value == 95)
                        {
                            WorldLink worldLink;

                            WorldLoader.SaveInterior(InteriorGenerator.CreateInterior(out worldLink, new Point(i, j)));

                            worldGrid[(worldGridChunk.X, worldGridChunk.Y)].AddWorldLink(worldLink);
                        }
                    }

                    if (Value <= 10 && Value > 6)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].AddAmbientObject(AmbientObjectFactory.createSmallRocks(new Vector2(i * WorldGrid.BlockSize.X, j * WorldGrid.BlockSize.Y)));
                    }
                    else if (Value <= 6 && Value >= 4)
                    {
                        AmbientHitableObject tree = AmbientObjectFactory.createTree(new Vector2(i * WorldGrid.BlockSize.X, j * WorldGrid.BlockSize.Y));

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].AddContainedObject(tree);
                    }
                }

            foreach(KeyValuePair<(int,int), WalkableGridChunk> walkableGridChunk in walkableGrid)
            {
                WorldLoader.SaveWalkableGridChunk(walkableGridChunk.Key.Item1, walkableGridChunk.Key.Item2, walkableGridChunk.Value);
            }

            foreach (KeyValuePair<(int, int), WorldGridChunk> worldGridChunk in worldGrid)
            {
                WorldLoader.SaveWorldGridChunk(worldGridChunk.Key.Item1, worldGridChunk.Key.Item2, worldGridChunk.Value);
            }
        }
    }
}
