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
    class World
    {
        private static Point BlockSize = new Point(32, 32);

        private Block[][] grid;
        private Point dimensions;

        public World()
        {
            dimensions = new Point(100, 100);

            Initialize();
        }

        public World(Point dimensions)
        {
            this.dimensions = dimensions;

            Initialize();
        }

        private void Initialize()
        {
            grid = new Block[dimensions.X][];

            for (int i = 0; i < dimensions.X; i++)
            {
                grid[i] = new Block[dimensions.Y];

                for (int j = 0; j < dimensions.Y; j++)
                {
                    grid[i][j] = new Block();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle visibleBounds = Rectangle.Empty;

            SimulationGame.getVisibleArea(out visibleBounds);

            for (int y=0;y < dimensions.Y; y++)
            {
                for (int x=0; x < dimensions.X; x++)
                {
                    Rectangle blockBounds = new Rectangle(x * BlockSize.X - 1, y * BlockSize.Y - 1, BlockSize.X + 1, BlockSize.Y + 1);

                    if(visibleBounds.Intersects(blockBounds))
                    {
                        Vector2 worldPosition = new Vector2(x * BlockSize.X, y * BlockSize.Y);

                        grid[x][y].Draw(spriteBatch, ref worldPosition);
                    }
                }
            }
        }
    }
}
