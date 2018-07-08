using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.Base.Entity;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Renderer
{
    public class WorldRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Point topLeft = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Left, SimulationGame.visibleArea.Top, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);
            Point bottomRight = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Right - 1, SimulationGame.visibleArea.Bottom - 1, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

            for (int blockX = topLeft.X; blockX < bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY < bottomRight.Y; blockY++)
                {
                    Point worldGridChunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, World.WorldGrid.WorldChunkBlockSize.X, World.WorldGrid.WorldChunkBlockSize.Y);
                    WorldGridChunk worldGridChunk = SimulationGame.world.getWorldGridChunk(worldGridChunkPosition.X, worldGridChunkPosition.Y);

                    BlockRenderer.Draw(spriteBatch, blockX * World.WorldGrid.BlockSize.X, blockY * World.WorldGrid.BlockSize.Y, worldGridChunk.getBlockType(blockX, blockY));
                }

            Point chunkTopLeft = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Left, SimulationGame.visibleArea.Top, World.WorldGrid.WorldChunkPixelSize.X, World.WorldGrid.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Right - 1, SimulationGame.visibleArea.Bottom - 1, World.WorldGrid.WorldChunkPixelSize.X, World.WorldGrid.WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if(SimulationGame.isDebug)
                    {
                        SimulationGame.primitiveDrawer.Rectangle(new Rectangle(chunkX * World.WorldGrid.WorldChunkPixelSize.X, chunkY * World.WorldGrid.WorldChunkPixelSize.X, World.WorldGrid.WorldChunkPixelSize.X, World.WorldGrid.WorldChunkPixelSize.Y), Color.Red);
                    }

                    WorldGridChunk worldGridChunk = SimulationGame.world.getWorldGridChunk(chunkX, chunkY);

                    if (worldGridChunk.ambientObjects != null)
                        foreach (AmbientObject ambientObject in worldGridChunk.ambientObjects)
                            AmbientObjectRenderer.Draw(spriteBatch, ambientObject);

                    if (worldGridChunk.containedObjects != null)
                        foreach (HitableObject containedObject in worldGridChunk.containedObjects)
                        {
                            InteractiveObjectRenderer.Draw(spriteBatch, containedObject);

                            if (containedObject is LivingEntity)
                            {
                                LivingEntityRenderer.Draw(spriteBatch, gameTime, (LivingEntity)containedObject);
                            }
                        }
                }
        }
    }
}
