using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.Factories;
using Simulation.Game.Hud;
using System;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public class World
    {
        public static Point BlockSize = new Point(32, 32);
        public static int RenderOuterBlockRange = 3;
        public static Point dimensions = new Point(100, 100);

        private Block[][] grid;
        
        public World()
        {
            Initialize();
        }

        private void Initialize()
        {
            Random random = new Random();

            grid = new Block[dimensions.X][];

            for (int i = 0; i < dimensions.X; i++)
            {
                grid[i] = new Block[dimensions.Y];

                for (int j = 0; j < dimensions.Y; j++)
                {
                    int Value = random.Next(0, 100);

                    if (Value <= 2)
                    {
                        grid[i][j] = new Block(new Point(i, j), BlockType.GRASS_WATERHOLE);
                    }
                    else
                    {
                        grid[i][j] = new Block(new Point(i, j));
                    }
                }
            }

            // Add static objects
            for (int i = 0; i < dimensions.X; i++)
                for (int j = 0; j < dimensions.Y; j++)
                {
                    if (grid[i][j].blockType == BlockType.GRASS_WATERHOLE) continue;

                    int Value = random.Next(0, 100);

                    if (Value == 10)
                    {
                        StaticBlockingObject tree = StaticObjectFactory.createTree(new Vector2(i * BlockSize.X, j * BlockSize.Y));

                        grid[i][j].ambientObjects.Add(tree);

                        addHitableObject(tree);
                    }
                    else if (Value < 4)
                    {
                        grid[i][j].ambientObjects.Add(StaticObjectFactory.createSmallRocks(new Vector2(i * BlockSize.X, j * BlockSize.Y)));
                    }
                }
        }

        public List<Block> getTouchedWorldBlocks(ref Rectangle rect)
        {
            List<Block> retList = new List<Block>();
            int worldWidth = dimensions.X * BlockSize.X;
            int worldHeight = dimensions.Y * BlockSize.Y;

            int left = Math.Max(0, rect.Left - rect.Left % BlockSize.X);
            int right = Math.Min(worldWidth, rect.Right + (BlockSize.X - rect.Right % BlockSize.X));

            int startTop = Math.Max(0, rect.Top - rect.Top % BlockSize.Y);
            int bottom = Math.Min(worldHeight, rect.Bottom + (BlockSize.Y - rect.Bottom % BlockSize.Y));

            for (; left < right; left += BlockSize.X)
                for (int top = startTop; top < bottom; top += BlockSize.Y)
                    retList.Add(grid[left / BlockSize.X][top / BlockSize.Y]);

            return retList;
        }

        public bool canMove(Rectangle rect)
        {
            int worldWidth = dimensions.X * BlockSize.X;
            int worldHeight = dimensions.Y * BlockSize.Y;

            int left = Math.Max(0, rect.Left - rect.Left % BlockSize.X);
            int right = Math.Min(worldWidth, rect.Right + (BlockSize.X - rect.Right % BlockSize.X));

            int startTop = Math.Max(0, rect.Top - rect.Top % BlockSize.Y);
            int bottom = Math.Min(worldHeight, rect.Bottom + (BlockSize.Y - rect.Bottom % BlockSize.Y));

            for (; left < right; left += BlockSize.X)
                for (int top = startTop; top < bottom; top += BlockSize.Y)
                {
                    Block block = grid[left / BlockSize.X][top / BlockSize.Y];

                    if (block.blockingType == BlockingType.BLOCKING)
                        return false;

                    foreach (HitableObject hitableObject in block.hitableObjects)
                        if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject.blockingBounds.Intersects(rect))
                        {
                            return false;
                        }
                }

            return true;
        }

        public void addHitableObject(HitableObject hitableObject)
        {
            if(hitableObject.useSameBounds)
            {
                List<Block> blocks = getTouchedWorldBlocks(ref hitableObject.hitBoxBounds);

                foreach (Block block in blocks)
                    block.hitableObjects.Add(hitableObject);
            }
            else
            {
                Rectangle union = Rectangle.Union(hitableObject.hitBoxBounds, hitableObject.blockingBounds);
                List<Block> blocks = getTouchedWorldBlocks(ref union);

                foreach (Block block in blocks)
                    block.hitableObjects.Add(hitableObject);
            }
        }

        public void removeHitableObject(HitableObject hitableObject)
        {
            if (hitableObject.useSameBounds)
            {
                List<Block> blocks = getTouchedWorldBlocks(ref hitableObject.hitBoxBounds);

                foreach (Block block in blocks)
                    block.hitableObjects.Remove(hitableObject);
            }
            else
            {
                Rectangle union = Rectangle.Union(hitableObject.hitBoxBounds, hitableObject.blockingBounds);
                List<Block> blocks = getTouchedWorldBlocks(ref union);

                foreach (Block block in blocks)
                    block.hitableObjects.Remove(hitableObject);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        { 
            int x = Math.Max(0, ((SimulationGame.visibleArea.Left - SimulationGame.visibleArea.Left % BlockSize.X) / BlockSize.X));
            int maxX = Math.Min(dimensions.X, ((SimulationGame.visibleArea.Right + (BlockSize.X - SimulationGame.visibleArea.Right % BlockSize.X)) / BlockSize.X));
            int startY = Math.Max(0, ((SimulationGame.visibleArea.Top - SimulationGame.visibleArea.Top % BlockSize.Y) / BlockSize.Y));
            int maxY = Math.Min(dimensions.Y, ((SimulationGame.visibleArea.Bottom + (BlockSize.Y - SimulationGame.visibleArea.Bottom % BlockSize.Y)) / BlockSize.Y));

            for (; x < maxX; x++)
            {
                for (int y = startY; y < maxY; y++)
                {
                    grid[x][y].Draw(spriteBatch);
                }
            }
        }
    }
}
