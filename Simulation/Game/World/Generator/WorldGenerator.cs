using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Factories;
using Simulation.Util;
using System;
using System.Collections.Generic;

namespace Simulation.Game.World.Generator
{
    public class WorldGenerator
    {
        private static Point generatedChunkBlockSize = new Point(128, 128);

        private Random random;
        private object generatorLock = new object();

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public void generateChunk(int blockX, int blockY)
        {
            lock(generatorLock)
            {
                // TODO: Check if chunk already exists

                generateWorld(blockX, blockY);
            }
        }

        public void resetWorld()
        {
            lock (generatorLock)
            {
                Util.Util.clearWorldFiles();
            }
        }

        private void generateWorld(int blockX, int blockY)
        {
            Point chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, generatedChunkBlockSize.X, generatedChunkBlockSize.Y);

            Dictionary<(int, int), WorldGridChunk> worldGrid = new Dictionary<(int, int), WorldGridChunk>();
            Dictionary<(int, int), WalkableGridChunk> walkableGrid = new Dictionary<(int, int), WalkableGridChunk>();

            var newX = chunkPosition.X * generatedChunkBlockSize.X;
            var newY = chunkPosition.Y * generatedChunkBlockSize.Y;

            // Loop over Blocks
            for (int i = newX; i < (newX + generatedChunkBlockSize.X); i++)
                for (int j = newY; j < (newY + generatedChunkBlockSize.Y); j++)
                {
                    Point worldGridChunk = GeometryUtils.getChunkPosition(i, j, World.WorldChunkBlockSize.X, World.WorldChunkBlockSize.Y);
                    Point walkableGridChunk = GeometryUtils.getChunkPosition(i, j, WalkableGrid.WalkableGridBlockChunkSize.X, WalkableGrid.WalkableGridBlockChunkSize.Y);

                    if(worldGrid.ContainsKey((worldGridChunk.X, worldGridChunk.Y)) == false)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)] = new WorldGridChunk(worldGridChunk.X * World.WorldChunkPixelSize.X, worldGridChunk.Y * World.WorldChunkPixelSize.Y);
                    }

                    if (walkableGrid.ContainsKey((walkableGridChunk.X, walkableGridChunk.Y)) == false)
                    {
                        walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)] = WalkableGridChunk.createEmpty();
                    }
                    
                    int Value = random.Next(0, 100);

                    if (Value <= 2)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].setBlockType(i, j, BlockType.GRASS_WATERHOLE);

                        WalkableGrid.changeBlockInChunk(walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)], i, j, true);
                    }
                    else
                    {
                        int randomTexture = random.Next((int)BlockType.GRASS_01, (int)BlockType.GRASS_04 + 1);

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].setBlockType(i, j, (BlockType)randomTexture);
                    }

                    if (Value <= 6 && Value >= 4)
                    {
                        StaticBlockingObject tree = StaticObjectFactory.createTree(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y));

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].addAmbientObject(tree);

                        //block.addAmbientObject(tree);
                    }
                    else if (Value < 4)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].addAmbientObject(StaticObjectFactory.createSmallRocks(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y)));
                    }
                }

            foreach(KeyValuePair<(int,int), WalkableGridChunk> walkableGridChunk in walkableGrid)
            {
                WorldLoader.saveWalkableGridChunk(walkableGridChunk.Key.Item1, walkableGridChunk.Key.Item2, walkableGridChunk.Value);
            }

            foreach (KeyValuePair<(int, int), WorldGridChunk> worldGridChunk in worldGrid)
            {
                WorldLoader.saveWorldGridChunk(worldGridChunk.Key.Item1, worldGridChunk.Key.Item2, worldGridChunk.Value);
            }
        }
    }
}
