using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using Simulation.Game.Factories;
using Simulation.Game.Renderer;
using System;

namespace Simulation.Game.World.Generation
{
    public class WorldGeneration
    {
        private static Random random = new Random();

        public static void addStaticObjects(Point dimensions)
        {
            // Add static objects
            for (int i = 0; i < dimensions.X; i++)
                for (int j = 0; j < dimensions.Y; j++)
                {
                    Block block = SimulationGame.world.GetBlock(i, j);

                    if (block.blockType == BlockRenderType.GRASS_WATERHOLE) continue;

                    int Value = random.Next(0, 100);

                    if (Value <= 6 && Value >= 4)
                    {
                        StaticBlockingObject tree = StaticObjectFactory.createTree(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y));

                        block.addAmbientObject(tree);

                        SimulationGame.world.addHitableObject(tree);
                    }
                    else if (Value < 4)
                    {
                        block.addAmbientObject(StaticObjectFactory.createSmallRocks(new Vector2(i * World.BlockSize.X, j * World.BlockSize.Y)));
                    }
                }
        }

        public static Block[,] generateWorld(Point dimensions)
        {
            Block[,] grid = new Block[dimensions.X, dimensions.Y];

            for (int i = 0; i < dimensions.X; i++)
                for (int j = 0; j < dimensions.Y; j++)
                {
                    int Value = random.Next(0, 100);

                    if (Value <= 2)
                    {
                        grid[i, j] = new Block(new Point(i, j), BlockRenderType.GRASS_WATERHOLE, BlockingType.BLOCKING);
                    }
                    else
                    {
                        int randomTexture = random.Next((int)BlockRenderType.GRASS_01, (int)BlockRenderType.GRASS_04 + 1);

                        grid[i, j] = new Block(new Point(i, j), (BlockRenderType)randomTexture);
                    }
                }

            return grid;
        }
    }
}
