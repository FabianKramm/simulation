using Microsoft.Xna.Framework;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.PathFinding;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulation.Game.Base.Entity
{
    public class MovingEntity: LivingEntity
    {
        private Task<List<GridPos>> findPathTask;
        private List<GridPos> walkPath;

        public Vector2 direction;
        private float velocity = 0.2f;

        // Create from JSON
        protected MovingEntity() {}

        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds):
            base(livingEntityType, position, relativeHitBoxBounds) {}

        public void WalkTo(int destBlockX, int destBlockY)
        {
            Point currentBlock = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

            findPathTask = PathFinder.FindPath(currentBlock.X, currentBlock.Y, destBlockX, destBlockY);

            walkPath = null;
            direction = Vector2.Zero;
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            ThreadingUtils.assertMainThread();

            if (canMove(newPosition))
            {
                disconnectFromWorld();

                // TODO: Check if we are moving into unloaded area => if yes then we load the tile and unload us
                base.UpdatePosition(newPosition);

                connectToWorld();
            }
        }

        private void connectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y))
                    SimulationGame.World.getWorldGridChunk(positionChunk.X, positionChunk.Y).AddContainedObject(this);

                // Add as interactive object for adjoined chunks
                Point chunkTopLeft = GeometryUtils.getChunkPosition(unionBounds.Left, unionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.getChunkPosition(unionBounds.Right - 1, unionBounds.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.getWorldGridChunk(chunkX, chunkY).AddOverlappingObject(this);
                        }
                    }

                SimulationGame.World.walkableGrid.addInteractiveObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.GetInterior(InteriorID).AddContainedObject(this);
            }
        }

        private void disconnectFromWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y))
                    SimulationGame.World.getWorldGridChunk(positionChunk.X, positionChunk.Y).RemoveContainedObject(this);

                Point chunkTopLeft = GeometryUtils.getChunkPosition(unionBounds.Left, unionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.getChunkPosition(unionBounds.Right - 1, unionBounds.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.getWorldGridChunk(chunkX, chunkY).RemoveOverlappingObject(this);
                        }
                    }

                SimulationGame.World.walkableGrid.removeInteractiveObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.GetInterior(InteriorID).RemoveContainedObject(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (findPathTask != null && findPathTask.IsCompleted)
            {
                walkPath = findPathTask.Result;

                if(walkPath != null && walkPath.Count > 1)
                {
                    walkPath.RemoveAt(0);
                }

                findPathTask = null;
            }

            if (walkPath != null)
            {
                var destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);

                if(Math.Abs(destPos.X - position.X) < GeometryUtils.SmallFloat && Math.Abs(destPos.Y - position.Y) < GeometryUtils.SmallFloat)
                {
                    if(walkPath.Count > 1)
                    {
                        walkPath.RemoveAt(0);
                        destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);
                    }
                    else
                    {
                        walkPath = null;
                        direction = Vector2.Zero;

                        destPos = Vector2.Zero;
                    }
                }

                if(destPos != Vector2.Zero)
                {
                    direction = new Vector2(destPos.X - position.X, destPos.Y - position.Y);
                    direction.Normalize();

                    Vector2 newPos = new Vector2(position.X + direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds, position.Y + direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds);

                    newPos.X = position.X < destPos.X ? Math.Min(destPos.X, newPos.X) : Math.Max(destPos.X, newPos.X);
                    newPos.Y = position.Y < destPos.Y ? Math.Min(destPos.Y, newPos.Y) : Math.Max(destPos.Y, newPos.Y);

                    UpdatePosition(newPos);
                }
            }
            else
            {
                if (direction != Vector2.Zero)
                {
                    UpdatePosition(new Vector2(position.X + direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds, position.Y + direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds));
                }
            }
        }
    }
}
