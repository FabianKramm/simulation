using Microsoft.Xna.Framework;
using Simulation.Game.AI;
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

        public bool IsWalking
        {
            get
            {
                return findPathTask != null || walkPath != null || Direction != Vector2.Zero;
            }
        }

        public Vector2 Direction;
        public float Velocity = 0.08f;

        public bool CanWalk = true;

        // Create from JSON
        protected MovingEntity() {}

        public MovingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds):
            base(livingEntityType, position, relativeHitBoxBounds) {}

        public MovingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, BaseAI baseAI) :
            base(livingEntityType, position, relativeHitBoxBounds)
        {
            SetAI(baseAI);
        }

        private void executeWorldLink()
        {
            WorldLink worldLink = null;

            if (InteriorID == Interior.Outside)
            {
                // Check if we are on a worldLink
                WorldGridChunk worldGridChunk = SimulationGame.World.GetWorldGridChunkFromReal((int)Position.X, (int)Position.Y);
                string key = BlockPosition.X + "," + BlockPosition.Y;

                if (worldGridChunk.WorldLinks != null && worldGridChunk.WorldLinks.ContainsKey(key) == true)
                {
                    worldLink = worldGridChunk.WorldLinks[key];
                }
            }
            else
            {
                // Check if we were on a worldLink
                Interior interior = SimulationGame.World.InteriorManager.GetInterior(InteriorID);
                string key = BlockPosition.X + "," + BlockPosition.Y;

                if (interior.WorldLinks != null && interior.WorldLinks.ContainsKey(key) == true)
                {
                    worldLink = interior.WorldLinks[key];
                }
            }

            if (worldLink != null)
            {
                Vector2 newPosition = new Vector2(worldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, worldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);

                base.UpdatePosition(new WorldPosition(newPosition, InteriorID));
            }
        }

        public void WalkTo(int destBlockX, int destBlockY)
        {
            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            findPathTask = PathFinder.FindPath(currentBlock.X, currentBlock.Y, destBlockX, destBlockY);

            walkPath = null;
            Direction = Vector2.Zero;
        }

        public override void UpdatePosition(WorldPosition newPosition)
        {
            ThreadingUtils.assertMainThread();

            if (!CanWalk) return;

            if (canMove(newPosition))
            {
                // TODO: Check if we are moving into unloaded area and we aren't a durable entity => if yes then we load the tile and unload us
                base.UpdatePosition(newPosition);
            }
            else
            {
                StopWalking();
            }
        }

        private bool walkIfWalkpath(GameTime gameTime)
        {
            if (findPathTask != null && findPathTask.IsCompleted)
            {
                if (findPathTask.Result != null && findPathTask.Result.Count > 1)
                {
                    walkPath = findPathTask.Result;
                    walkPath.RemoveAt(0);
                }

                findPathTask = null;
            }

            if (walkPath != null)
            {
                var destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);

                if (Math.Abs(destPos.X - Position.X) < GeometryUtils.SmallFloat && Math.Abs(destPos.Y - Position.Y) < GeometryUtils.SmallFloat)
                {
                    if (walkPath.Count > 1)
                    {
                        walkPath.RemoveAt(0);
                        destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);
                    }
                    else
                    {
                        StopWalking();

                        // We call this because we now want to check if we are on a world link
                        executeWorldLink();
                        return true;
                    }
                }

                if (destPos != Vector2.Zero)
                {
                    Direction = new Vector2(destPos.X - Position.X, destPos.Y - Position.Y);
                    Direction.Normalize();

                    WorldPosition newPos = new WorldPosition(Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds);

                    newPos.X = Position.X < destPos.X ? Math.Min(destPos.X, newPos.X) : Math.Max(destPos.X, newPos.X);
                    newPos.Y = Position.Y < destPos.Y ? Math.Min(destPos.Y, newPos.Y) : Math.Max(destPos.Y, newPos.Y);

                    UpdatePosition(newPos);
                }

                return true;
            }

            return false;
        }

        public void StopWalking()
        {
            findPathTask = null;
            walkPath = null;

            Direction = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if(walkIfWalkpath(gameTime) == false)
            {
                if (Direction != Vector2.Zero)
                {
                    UpdatePosition(new WorldPosition(Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds, Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds));

                    executeWorldLink();
                }
            }

            base.Update(gameTime);
        }
    }
}
