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

        private bool executeWorldLink(WorldPosition newPosition = null)
        {
            WorldLink oldWorldLink = SimulationGame.World.GetWorldLinkFromPosition(Position);
            WorldLink newWorldLink = newPosition != null ? SimulationGame.World.GetWorldLinkFromPosition(newPosition) : null;

            if (oldWorldLink == null && newWorldLink != null)
            {
                Vector2 newWorldPosition = new Vector2(newWorldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, newWorldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);

                base.UpdatePosition(new WorldPosition(newWorldPosition, newWorldLink.ToInteriorID));

                return true;
            }

            return false;
        }

        public void WalkTo(WorldPosition worldPosition)
        {
            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            // TODO: WHAT HAPPENS IF WE CHANGE INTERIOR
            findPathTask = PathFinder.FindPath(new WorldPosition(currentBlock.X, currentBlock.Y, Position.InteriorID), worldPosition);

            walkPath = null;
            Direction = Vector2.Zero;
        }

        protected override void UpdatePosition(WorldPosition newPosition)
        {
            // TODO: Check if we are moving into unloaded area and we aren't a durable entity => if yes then we load the tile and unload us
            base.UpdatePosition(newPosition);
        }

        public void StopWalking()
        {
            findPathTask = null;
            walkPath = null;

            Direction = Vector2.Zero;
        }

        private void loadWalkpath(GameTime gameTime)
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
        }

        private void updatePositionWithOverflow(float newPosX, float newPosY)
        {
            UpdatePosition(new WorldPosition((int)Math.Round(newPosX), (int)Math.Round(newPosY)));
        }

        public override void Update(GameTime gameTime)
        {
            loadWalkpath(gameTime);

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
                    }
                }

                if (destPos != Vector2.Zero)
                {
                    Direction = new Vector2(destPos.X - Position.X, destPos.Y - Position.Y);
                    Direction.Normalize();

                    // TODO: OVERFLOW
                    float newPosX = Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds;
                    float newPosY = Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds;

                    newPosX = Position.X < destPos.X ? Math.Min(destPos.X, newPosX) : Math.Max(destPos.X, newPosX);
                    newPosY = Position.Y < destPos.Y ? Math.Min(destPos.Y, newPosY) : Math.Max(destPos.Y, newPosY);

                    var newPos = new WorldPosition((int)Math.Round(newPosX), (int)Math.Round(newPosY), InteriorID);

                    if (CanWalk && canMove(newPos))
                    {
                        UpdatePosition(newPos);
                    }
                    else
                    {
                        StopWalking();
                    }
                }
            }
            else
            {
                if (Direction != Vector2.Zero)
                {
                    // TODO: OVERFLOW
                    float newPosX = Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds;
                    float newPosY = Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds;
                    var newPos = new WorldPosition((int)Math.Round(newPosX), (int)Math.Round(newPosY), InteriorID);

                    if (CanWalk && canMove(newPos))
                    {
                        var executedWorldLink = executeWorldLink(newPos);

                        if (executedWorldLink == false)
                            UpdatePosition(newPos);
                    }
                    else
                    {
                        StopWalking();
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
