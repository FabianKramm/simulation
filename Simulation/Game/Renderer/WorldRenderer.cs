using Microsoft.Xna.Framework.Graphics;
using System;

namespace Simulation.Game.Renderer
{
    public class WorldRenderer
    {
        public static void Draw(SpriteBatch spriteBatch)
        {
            int x = Math.Max(0, ((SimulationGame.visibleArea.Left - SimulationGame.visibleArea.Left % World.World.BlockSize.X) / World.World.BlockSize.X));
            int maxX = Math.Min(World.World.dimensions.X, ((SimulationGame.visibleArea.Right + (World.World.BlockSize.X - SimulationGame.visibleArea.Right % World.World.BlockSize.X)) / World.World.BlockSize.X));
            int startY = Math.Max(0, ((SimulationGame.visibleArea.Top - SimulationGame.visibleArea.Top % World.World.BlockSize.Y) / World.World.BlockSize.Y));
            int maxY = Math.Min(World.World.dimensions.Y, ((SimulationGame.visibleArea.Bottom + (World.World.BlockSize.Y - SimulationGame.visibleArea.Bottom % World.World.BlockSize.Y)) / World.World.BlockSize.Y));

            for (; x < maxX; x++)
            {
                for (int y = startY; y < maxY; y++)
                {
                    BlockRenderer.Draw(spriteBatch, SimulationGame.world.GetBlock(x, y));
                }
            }
        }
    }
}
