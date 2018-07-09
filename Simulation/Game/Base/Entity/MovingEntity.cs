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

        public Vector2 Direction;
        public float Velocity = 0.2f;

        // Create from JSON
        protected MovingEntity() {}

        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds):
            base(livingEntityType, position, relativeHitBoxBounds) {}

        public void WalkTo(int destBlockX, int destBlockY)
        {
            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

            findPathTask = PathFinder.FindPath(currentBlock.X, currentBlock.Y, destBlockX, destBlockY);

            walkPath = null;
            Direction = Vector2.Zero;
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            ThreadingUtils.assertMainThread();

            WorldLink worldLink = null;

            if(InteriorID == Interior.Outside)
            {
                // Check if we move to a world link
                WorldGridChunk worldGridChunk = WorldGridChunk.GetWorldGridChunk((int)newPosition.X, (int)newPosition.Y);
                Point newBlockPosition = GeometryUtils.GetBlockFromReal((int)newPosition.X, (int)newPosition.Y);

                if (worldGridChunk.WorldLinks != null && worldGridChunk.WorldLinks.ContainsKey(newBlockPosition))
                {
                    worldLink = worldGridChunk.WorldLinks[newBlockPosition];
                }
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.GetInterior(InteriorID);
                Point newBlockPosition = GeometryUtils.GetBlockFromReal((int)newPosition.X, (int)newPosition.Y);

                if (interior.WorldLinks != null && interior.WorldLinks.ContainsKey(newBlockPosition))
                {
                    worldLink = interior.WorldLinks[newBlockPosition];
                }
            }

            if(worldLink != null)
            {
                disconnectFromWorld();

                newPosition = new Vector2(worldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, worldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);

                InteriorID = worldLink.ToInteriorID;
                base.UpdatePosition(newPosition);

                connectToWorld();
            }
            else
            {
                if (canMove(newPosition))
                {
                    disconnectFromWorld();

                    // TODO: Check if we are moving into unloaded area => if yes then we load the tile and unload us
                    base.UpdatePosition(newPosition);

                    connectToWorld();
                }
            }
        }

        private void connectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y))
                    SimulationGame.World.GetWorldGridChunk(positionChunk.X, positionChunk.Y).AddContainedObject(this);

                // Add as interactive object for adjoined chunks
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right - 1, UnionBounds.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.GetWorldGridChunk(chunkX, chunkY).AddOverlappingObject(this);
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
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y))
                    SimulationGame.World.GetWorldGridChunk(positionChunk.X, positionChunk.Y).RemoveContainedObject(this);

                Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right - 1, UnionBounds.Bottom - 1, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.GetWorldGridChunk(chunkX, chunkY).RemoveOverlappingObject(this);
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

                if(Math.Abs(destPos.X - Position.X) < GeometryUtils.SmallFloat && Math.Abs(destPos.Y - Position.Y) < GeometryUtils.SmallFloat)
                {
                    if(walkPath.Count > 1)
                    {
                        walkPath.RemoveAt(0);
                        destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);
                    }
                    else
                    {
                        walkPath = null;
                        Direction = Vector2.Zero;

                        destPos = Vector2.Zero;
                    }
                }

                if(destPos != Vector2.Zero)
                {
                    Direction = new Vector2(destPos.X - Position.X, destPos.Y - Position.Y);
                    Direction.Normalize();

                    Vector2 newPos = new Vector2(Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds);

                    newPos.X = Position.X < destPos.X ? Math.Min(destPos.X, newPos.X) : Math.Max(destPos.X, newPos.X);
                    newPos.Y = Position.Y < destPos.Y ? Math.Min(destPos.Y, newPos.Y) : Math.Max(destPos.Y, newPos.Y);

                    UpdatePosition(newPos);
                }
            }
            else
            {
                if (Direction != Vector2.Zero)
                {
                    UpdatePosition(new Vector2(Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds));
                }
            }
        }
    }
}
