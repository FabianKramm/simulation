using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Renderer
{
    public class WorldRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Point topLeft = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Left, SimulationGame.visibleArea.Top, World.World.BlockSize.X, World.World.BlockSize.Y);
            Point bottomRight = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Right, SimulationGame.visibleArea.Bottom, World.World.BlockSize.X, World.World.BlockSize.Y);

            for (int blockX = topLeft.X; blockX < bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY < bottomRight.Y; blockY++)
                {
                    Point worldGridChunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, World.World.WorldChunkBlockSize.X, World.World.WorldChunkBlockSize.Y);
                    WorldGridChunk worldGridChunk = SimulationGame.world.getWorldGridChunk(worldGridChunkPosition.X, worldGridChunkPosition.Y);

                    BlockRenderer.Draw(spriteBatch, blockX * World.World.BlockSize.X, blockY * World.World.BlockSize.Y, worldGridChunk.getBlockType(blockX, blockY));
                }

            Point chunkTopLeft = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Left, SimulationGame.visibleArea.Top, World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.getChunkPosition(SimulationGame.visibleArea.Right, SimulationGame.visibleArea.Bottom, World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if(SimulationGame.isDebug)
                    {
                        SimulationGame.primitiveDrawer.Rectangle(new Rectangle(chunkX * World.World.WorldChunkPixelSize.X, chunkY * World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.Y), Color.Red);
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
