using Microsoft.Xna.Framework;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.PathFinding;
using Simulation.Util;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulation.Game.Objects.Entities
{
    public class MovingEntity: LivingEntity
    {
        private Task<List<GridPos>> findPathTask;
        private List<GridPos> walkPath;

        public Vector2 Direction;
        public float Velocity = 0.2f;

        public bool CanWalk = true;

        // Create from JSON
        protected MovingEntity() {}

        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rect relativeHitBoxBounds):
            base(livingEntityType, position, relativeHitBoxBounds) {}

        public void WalkTo(int destBlockX, int destBlockY)
        {
            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            findPathTask = PathFinder.FindPath(currentBlock.X, currentBlock.Y, destBlockX, destBlockY);

            walkPath = null;
            Direction = Vector2.Zero;
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            ThreadingUtils.assertMainThread();

            if (!CanWalk) return;

            WorldLink worldLink = null;

            if(InteriorID == Interior.Outside)
            {
                // Check if we were on a worldLink
                WorldGridChunk oldWorldGridChunk = SimulationGame.World.GetWorldGridChunkFromReal((int)Position.X, (int)Position.Y);
                string oldKey = BlockPosition.X + "," + BlockPosition.Y;

                if (oldWorldGridChunk.WorldLinks == null || oldWorldGridChunk.WorldLinks.ContainsKey(oldKey) == false)
                {
                    // Check if we move to a world link
                    WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunkFromReal((int)newPosition.X, (int)newPosition.Y);
                    Point newBlockPosition = GeometryUtils.GetBlockFromReal((int)newPosition.X, (int)newPosition.Y);
                    string key = newBlockPosition.X + "," + newBlockPosition.Y;

                    if (worldGridChunk.WorldLinks != null && worldGridChunk.WorldLinks.ContainsKey(key))
                    {
                        worldLink = worldGridChunk.WorldLinks[key];
                    }
                }
            }
            else
            {
                // Check if we were on a worldLink
                Interior interior = SimulationGame.World.InteriorManager.GetInterior(InteriorID);
                string oldKey = BlockPosition.X + "," + BlockPosition.Y;

                if (interior.WorldLinks == null || interior.WorldLinks.ContainsKey(oldKey) == false)
                {
                    Point newBlockPosition = GeometryUtils.GetBlockFromReal((int)newPosition.X, (int)newPosition.Y);
                    string key = newBlockPosition.X + "," + newBlockPosition.Y;

                    if (interior.WorldLinks != null && interior.WorldLinks.ContainsKey(key))
                    {
                        worldLink = interior.WorldLinks[key];
                    }
                }
            }

            if(worldLink != null)
            {
                DisconnectFromWorld();

                newPosition = new Vector2(worldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, worldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);

                InteriorID = worldLink.ToInteriorID;
                base.UpdatePosition(newPosition);

                // Here we should load the interior asynchronously
                ConnectToWorld();
            }
            else
            {
                if (canMove(newPosition))
                {
                    DisconnectFromWorld();

                    // TODO: Check if we are moving into unloaded area => if yes then we load the tile and unload us
                    base.UpdatePosition(newPosition);

                    ConnectToWorld();
                }
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
