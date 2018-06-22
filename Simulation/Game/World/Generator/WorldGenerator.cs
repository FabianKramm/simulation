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
        private static Point generatedChunkSize = new Point(576, 576);

        private Random random;
        private object generatorLock = new object();

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public void generateChunk(int x, int y)
        {
            lock(generatorLock)
            {
                generateWorld(x, y);
            }
        }

        public void resetWorld()
        {
            lock (generatorLock)
            {
                Util.Util.clearWorldFiles();
            }
        }

        private void generateWorld(int x, int y)
        {
            Point chunkPosition = GeometryUtils.projectPosition(x, y, generatedChunkSize.X, generatedChunkSize.Y);

            Dictionary<(int, int), WorldGridChunk> worldGrid = new Dictionary<(int, int), WorldGridChunk>();
            Dictionary<(int, int), UInt32[]> walkableGrid = new Dictionary<(int, int), UInt32[]>();

            var newX = x * generatedChunkSize.X;
            var newY = y * generatedChunkSize.Y;

            for (int i = newX; i < (newX + generatedChunkSize.X); i++)
                for (int j = newY; j < (newY + generatedChunkSize.Y); j++)
                {
                    Point worldGridChunk = GeometryUtils.projectPosition(i, j, World.WorldChunkSize.X, World.WorldChunkSize.Y);
                    Point walkableGridChunk = GeometryUtils.projectPosition(i, j, WalkableGrid.WalkableGridChunkSize.X, WalkableGrid.WalkableGridChunkSize.Y);

                    if(worldGrid.ContainsKey((worldGridChunk.X, worldGridChunk.Y)) == false)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)] = new WorldGridChunk();
                    }

                    if (walkableGrid.ContainsKey((walkableGridChunk.X, walkableGridChunk.Y)) == false)
                    {
                        walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)] = new UInt32[WalkableGrid.WalkableGridChunkCount];
                    }
                    
                    int Value = random.Next(0, 100);

                    if (Value <= 2)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].setBlockType(i, j, BlockType.GRASS_WATERHOLE);
                        WalkableGrid.changePositionInChunk(walkableGrid[(walkableGridChunk.X, walkableGridChunk.Y)], i, j, true);
                    }
                    else
                    {
                        int randomTexture = random.Next((int)BlockType.GRASS_01, (int)BlockType.GRASS_04 + 1);

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].setBlockType(i, j, (BlockType)randomTexture);
                    }

                    if (Value <= 6 && Value >= 4)
                    {
                        StaticBlockingObject tree = StaticObjectFactory.createTree(new Vector2(i, j));

                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].addAmbientObject(tree);

                        //block.addAmbientObject(tree);
                    }
                    else if (Value < 4)
                    {
                        worldGrid[(worldGridChunk.X, worldGridChunk.Y)].addAmbientObject(StaticObjectFactory.createSmallRocks(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y)));

                        // block.addAmbientObject(StaticObjectFactory.createSmallRocks(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y)));
                    }
                }

            foreach(KeyValuePair<(int,int), UInt32[]> walkableGridChunk in walkableGrid)
            {
                WalkableGrid.saveChunk(walkableGridChunk.Key.Item1, walkableGridChunk.Key.Item2, walkableGridChunk.Value);
            }

            foreach (KeyValuePair<(int, int), WorldGridChunk> worldGridChunk in worldGrid)
            {
                // World.saveChunk(worldGridChunk.Key.Item1, worldGridChunk.Key.Item2, worldGridChunk.Value);
            }
        }
    }
}
