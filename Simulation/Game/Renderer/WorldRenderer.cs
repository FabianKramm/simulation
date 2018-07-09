using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.Base.Entity;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util;
using System;

namespace Simulation.Game.Renderer
{
    public class WorldRenderer
    {
        private static void drawOutside(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Point topLeft = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Left, SimulationGame.VisibleArea.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point bottomRight = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Right - 1, SimulationGame.VisibleArea.Bottom - 1, WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

            for (int blockX = topLeft.X; blockX < bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY < bottomRight.Y; blockY++)
                {
                    Point worldGridChunkPosition = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                    WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunk(worldGridChunkPosition.X, worldGridChunkPosition.Y);

                    BlockRenderer.Draw(spriteBatch, blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, worldGridChunk.GetBlockType(blockX, blockY));
                }

            Point chunkTopLeft = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Left, SimulationGame.VisibleArea.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Right - 1, SimulationGame.VisibleArea.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if (SimulationGame.IsDebug)
                    {
                        SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(chunkX * WorldGrid.WorldChunkPixelSize.X, chunkY * WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y), Color.Red);
                    }

                    WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunk(chunkX, chunkY);

                    if (worldGridChunk.AmbientObjects != null)
                        foreach (AmbientObject ambientObject in worldGridChunk.AmbientObjects)
                            AmbientObjectRenderer.Draw(spriteBatch, ambientObject);

                    if (worldGridChunk.ContainedObjects != null)
                        foreach (HitableObject containedObject in worldGridChunk.ContainedObjects)
                        {
                            if (containedObject is LivingEntity)
                            {
                                LivingEntityRenderer.Draw(spriteBatch, gameTime, (LivingEntity)containedObject);
                            }
                            else
                            {
                                InteractiveObjectRenderer.Draw(spriteBatch, containedObject);
                            }
                        }

                    if (SimulationGame.IsDebug && worldGridChunk.WorldLinks != null)
                        foreach (var worldLinkItem in worldGridChunk.WorldLinks)
                            if (SimulationGame.VisibleArea.Contains(new Point(worldLinkItem.Value.FromBlock.X * WorldGrid.BlockSize.X, worldLinkItem.Value.FromBlock.Y * WorldGrid.BlockSize.Y)))
                                SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(worldLinkItem.Value.FromBlock.X * WorldGrid.BlockSize.X, worldLinkItem.Value.FromBlock.Y * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y), Color.DarkBlue);
                }
        }

        private static void drawInterior(SpriteBatch spriteBatch, GameTime gameTime, Interior interior)
        {
            Point topLeft = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Left, SimulationGame.VisibleArea.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point bottomRight = GeometryUtils.GetChunkPosition(SimulationGame.VisibleArea.Right - 1, SimulationGame.VisibleArea.Bottom - 1, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            topLeft.X = Math.Max(0, topLeft.X);
            topLeft.Y = Math.Max(0, topLeft.Y);

            bottomRight.X = Math.Min(interior.Dimensions.X, bottomRight.X);
            bottomRight.Y = Math.Min(interior.Dimensions.Y, bottomRight.Y);

            for (int blockX = topLeft.X; blockX < bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY < bottomRight.Y; blockY++)
                {
                    BlockRenderer.Draw(spriteBatch, blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, interior.GetBlockType(blockX, blockY));
                }

            if (interior.AmbientObjects != null)
                foreach (AmbientObject ambientObject in interior.AmbientObjects)
                    AmbientObjectRenderer.Draw(spriteBatch, ambientObject);

            if (interior.ContainedObjects != null)
                foreach (HitableObject containedObject in interior.ContainedObjects)
                {
                    if (containedObject is LivingEntity)
                    {
                        LivingEntityRenderer.Draw(spriteBatch, gameTime, (LivingEntity)containedObject);
                    }
                    else
                    {
                        InteractiveObjectRenderer.Draw(spriteBatch, containedObject);
                    }
                }

            if(SimulationGame.IsDebug && interior.WorldLinks != null)
                foreach(var worldLinkItem in interior.WorldLinks)
                    if(SimulationGame.VisibleArea.Contains(new Point(worldLinkItem.Value.FromBlock.X * WorldGrid.BlockSize.X, worldLinkItem.Value.FromBlock.Y * WorldGrid.BlockSize.Y)))
                        SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(worldLinkItem.Value.FromBlock.X * WorldGrid.BlockSize.X, worldLinkItem.Value.FromBlock.Y * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y), Color.DarkBlue);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.Player.InteriorID == Interior.Outside)
            {
                drawOutside(spriteBatch, gameTime);
            }
            else
            {
                drawInterior(spriteBatch, gameTime, SimulationGame.World.InteriorManager.GetInterior(SimulationGame.Player.InteriorID));
            }
        }
    }
}
