using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Enums;
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

        public Vector2 Direction;
        private WorldPosition destPosition;
        private List<GridPos> walkPath;

        public bool IsWalking
        {
            get
            {
                return destPosition != null || findPathTask != null || walkPath != null || Direction != Vector2.Zero;
            }
        }

        
        public float Velocity = 0.08f;

        public bool CanWalk = true;

        // Create from JSON
        protected MovingEntity() {}

        public MovingEntity(LivingEntityType livingEntityType, WorldPosition position, FractionType fraction) :
            base(livingEntityType, position, new Rect(-14, -48, 28, 48), fraction)
        {
            SetBlockingBounds(new Rect(-8, -20, 16, 20));
        }

        public MovingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, FractionType fraction) :
            base(livingEntityType, position, relativeHitBoxBounds, fraction)
        { }

        public MovingEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, BaseAI baseAI, FractionType fraction) :
            base(livingEntityType, position, relativeHitBoxBounds, fraction)
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
                UpdatePosition(new WorldPosition(newWorldPosition, newWorldLink.ToInteriorID));

                return true;
            }

            return false;
        }

        public void StopWalking()
        {
            findPathTask = null;
            walkPath = null;
            destPosition = null;

            Direction = Vector2.Zero;
        }

        public void WalkToPosition(WorldPosition realPosition)
        {
            StopWalking();

            if(realPosition.InteriorID == Position.InteriorID && (realPosition.X != Position.X || realPosition.Y != Position.Y))
            {
                destPosition = realPosition.Clone();
            }
        }

        public void WalkToBlock(Point blockPosition)
        {
            StopWalking();

            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            if(currentBlock.X != blockPosition.X || currentBlock.Y != blockPosition.Y)
            {
                findPathTask = PathFinder.FindPath(new WorldPosition(currentBlock.X, currentBlock.Y, Position.InteriorID), new WorldPosition(blockPosition.X, blockPosition.Y, Position.InteriorID));

                walkPath = null;
                Direction = Vector2.Zero;
            }
        }

        public void WalkToBlock(WorldPosition blockPosition)
        {
            StopWalking();

            Point currentBlock = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            if (Position.InteriorID != blockPosition.InteriorID || currentBlock.X != blockPosition.X || currentBlock.Y != blockPosition.Y)
            {
                // TODO: WHAT HAPPENS IF WE CHANGE INTERIOR => AI TOPIC?
                findPathTask = PathFinder.FindPath(new WorldPosition(currentBlock.X, currentBlock.Y, Position.InteriorID), blockPosition);

                walkPath = null;
                Direction = Vector2.Zero;
            }
        }

        protected override void UpdatePosition(WorldPosition newPosition)
        {
            // TODO: Check if we are moving into unloaded area and we aren't a durable entity => if yes then we load the tile and unload us
            base.UpdatePosition(newPosition);
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

        private bool changePosition(GameTime gameTime, Vector2 destPos)
        {
            float newPosX = Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds;
            float newPosY = Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds;

            newPosX = Position.X < destPos.X ? Math.Min(destPos.X, newPosX) : Math.Max(destPos.X, newPosX);
            newPosY = Position.Y < destPos.Y ? Math.Min(destPos.Y, newPosY) : Math.Max(destPos.Y, newPosY);

            var newPos = new WorldPosition(newPosX, newPosY, InteriorID);

            if (CanWalk && canMove(newPos))
            {
                UpdatePosition(newPos);

                return true;
            }

            return false;
        }

        public override void Update(GameTime gameTime)
        {
            loadWalkpath(gameTime);

            if (walkPath != null)
            {
                var destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 31);

                // Check if we are at position
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
                        WorldLink newWorldLink = SimulationGame.World.GetWorldLinkFromPosition(Position);

                        if(newWorldLink != null)
                        {
                            Vector2 newWorldPosition = new Vector2(newWorldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, newWorldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);
                            UpdatePosition(new WorldPosition(newWorldPosition, newWorldLink.ToInteriorID));
                        }
                    }
                }

                if (IsWalking && destPos != Vector2.Zero)
                {
                    Direction = new Vector2(destPos.X - Position.X, destPos.Y - Position.Y);
                    Direction.Normalize();

                    bool couldWalk = changePosition(gameTime, destPos);

                    if(!couldWalk)
                    {
                        StopWalking();
                    }
                }
            }
            else if(destPosition != null)
            {
                if(Direction == Vector2.Zero)
                {
                    Direction = new Vector2(destPosition.X - Position.X, destPosition.Y - Position.Y);
                    Direction.Normalize();
                }

                bool couldWalk = changePosition(gameTime, destPosition.ToVector());

                // Check if we couldn't move to position
                if (!couldWalk)
                {
                    Point destBlock = GeometryUtils.GetChunkPosition((int)destPosition.X, (int)destPosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                    WalkToBlock(destBlock);
                }
                else if (Math.Abs(destPosition.X - Position.X) < GeometryUtils.SmallFloat && Math.Abs(destPosition.Y - Position.Y) < GeometryUtils.SmallFloat)
                {
                    StopWalking();

                    // We call this because we now want to check if we are on a world link
                    WorldLink newWorldLink = SimulationGame.World.GetWorldLinkFromPosition(Position);

                    if (newWorldLink != null)
                    {
                        Vector2 newWorldPosition = new Vector2(newWorldLink.ToBlock.X * WorldGrid.BlockSize.X + WorldGrid.BlockSize.X / 2, newWorldLink.ToBlock.Y * WorldGrid.BlockSize.Y + WorldGrid.BlockSize.Y - 1);
                        UpdatePosition(new WorldPosition(newWorldPosition, newWorldLink.ToInteriorID));
                    }
                }
            }
            else
            {
                if (Direction != Vector2.Zero)
                {
                    float newPosX = Position.X + Direction.X * Velocity * gameTime.ElapsedGameTime.Milliseconds;
                    float newPosY = Position.Y + Direction.Y * Velocity * gameTime.ElapsedGameTime.Milliseconds;
                    var newPos = new WorldPosition(newPosX, newPosY, InteriorID);

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
