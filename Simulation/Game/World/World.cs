using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public class World
    {
        public static Point BlockSize = new Point(32, 32);

        private Block[][] grid;
        private Point dimensions;

        public World()
        {
            dimensions = new Point(1000, 1000);

            Initialize();
        }

        public World(Point dimensions)
        {
            this.dimensions = dimensions;

            Initialize();
        }

        public static List<(int x, int y)> getTouchedWorldBlocksCoordinates(Rectangle rect)
        {
            List<(int x, int y)> retList = new List<(int x, int y)>();
            int worldWidth = 100 * BlockSize.X;
            int worldHeight = 100 * BlockSize.Y;

            int moduloX = rect.Left % BlockSize.X;
            int moduloY = rect.Top % BlockSize.Y;

            for (int left = rect.Left; left <= worldWidth && left < rect.Right; left += BlockSize.X)
            {
                if (left >= 0)
                {
                    for (int top = rect.Top; top <= worldHeight && top < rect.Bottom; top += BlockSize.Y)
                    {
                        if (top >= 0)
                        {
                            retList.Add(((left - moduloX) / BlockSize.X, (top - moduloY) / BlockSize.Y));
                        }
                    }
                }
            }

            return retList;
        }

        public List<Block> getTouchedWorldBlocks(Rectangle rect)
        {
            List<Block> retList = new List<Block>();
            int worldWidth = dimensions.X * BlockSize.X;
            int worldHeight = dimensions.Y * BlockSize.Y;

            int left = rect.Left - rect.Left % BlockSize.X;
            int right = rect.Right + (BlockSize.X - rect.Right % BlockSize.X);
            int bottom = rect.Bottom + (BlockSize.Y - rect.Bottom % BlockSize.Y);

            for (;left<=worldWidth && left < right; left += BlockSize.X)
            {
                if(left >= 0)
                {
                    for (int top = rect.Top - rect.Top % BlockSize.Y; top <= worldHeight && top < bottom; top += BlockSize.Y)
                    {
                        if (top >= 0)
                        {
                            retList.Add(grid[left / BlockSize.X][top / BlockSize.Y]);
                        }
                    }
                }
            }

            return retList;
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

                    if(Value <= 4)
                    {
                        grid[i][j] = new Block(new Point(i, j), BlockType.GRASS_WATERHOLE);
                    }
                    else
                    {
                        grid[i][j] = new Block(new Point(i, j));
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        { 
            int x = Math.Max(0, (SimulationGame.visibleArea.Left - SimulationGame.visibleArea.Left % BlockSize.X) / BlockSize.X);
            int maxX = Math.Min(dimensions.X, (SimulationGame.visibleArea.Right + (BlockSize.X - SimulationGame.visibleArea.Right % BlockSize.X)) / BlockSize.X);
            int startY = Math.Max(0, (SimulationGame.visibleArea.Top - SimulationGame.visibleArea.Top % BlockSize.Y) / BlockSize.Y);
            int maxY = Math.Min(dimensions.Y, (SimulationGame.visibleArea.Bottom + (BlockSize.Y - SimulationGame.visibleArea.Bottom % BlockSize.Y)) / BlockSize.Y);

            for (; x < maxX; x++)
            {
                for (int y= startY; y < maxY; y++)
                {
                    grid[x][y].Draw(spriteBatch);
                }
            }
        }
    }
}
