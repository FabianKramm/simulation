using Microsoft.Xna.Framework;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.PathFinding;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simulation.Game.Objects.Entities
{
    public class MovingEntity: LivingEntity
    {
        [Serialize]
        public float Velocity = 0.10f;

        [Serialize]
        public bool CanWalk = true;

        [Serialize]
        public WorldPosition DestRealPosition { get; private set; }

        [Serialize]
        public WorldPosition DestBlockPosition { get; private set; }

        [Serialize]
        public Vector2 Direction;

        public bool IsWalking
        {
            get
            {
                return DestBlockPosition != null || DestRealPosition != null || findPathTask != null || walkPath != null || Direction != Vector2.Zero;
            }
        }

        private List<GridPos> walkPath;
        private Task<List<GridPos>> findPathTask;

        // Create from JSON
        protected MovingEntity() : base() { }

        public MovingEntity(WorldPosition worldPosition): base(worldPosition) {}

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
            DestRealPosition = null;
            DestBlockPosition = null;

            Direction = Vector2.Zero;
        }

        public void WalkToPosition(WorldPosition realPosition)
        {
            if (DestRealPosition != null && DestRealPosition.X == realPosition.X && DestRealPosition.Y == realPosition.Y && DestRealPosition.InteriorID == realPosition.InteriorID)
                return;

            if(realPosition.InteriorID != Position.InteriorID || realPosition.X != Position.X || realPosition.Y != Position.Y)
            {
                WorldPosition blockPosition = realPosition.ToBlockPosition();

                if (realPosition.InteriorID != Position.InteriorID)
                {
                    WalkToBlock(blockPosition);
                }
                else
                {
                    if (DestBlockPosition != null && DestBlockPosition.X == blockPosition.X && DestBlockPosition.Y == blockPosition.Y && DestBlockPosition.InteriorID == realPosition.InteriorID)
                        return;

                    StopWalking();
                    DestRealPosition = realPosition.Clone();
                }
            }
        }

        public void WalkToBlock(WorldPosition blockPosition)
        {
            if (DestBlockPosition != null && DestBlockPosition.X == blockPosition.X && DestBlockPosition.Y == blockPosition.Y && DestBlockPosition.InteriorID == blockPosition.InteriorID)
                return;

            StopWalking();

            Point currentBlock = Position.ToBlockPositionPoint();

            if (Position.InteriorID != blockPosition.InteriorID || currentBlock.X != blockPosition.X || currentBlock.Y != blockPosition.Y)
            {
                DestBlockPosition = blockPosition;

                walkPath = null;
                Direction = Vector2.Zero;
            }
        }

        private void loadWalkpath(GameTime gameTime)
        {
            if (DestBlockPosition != null && walkPath == null && findPathTask == null)
            {
                // TODO: WHAT HAPPENS IF WE CHANGE INTERIOR => AI TOPIC?
                findPathTask = PathFinder.FindPath(Position.ToBlockPosition(), DestBlockPosition);
            }


            if (findPathTask != null && findPathTask.IsCompleted)
            {
                if (findPathTask.Result != null && findPathTask.Result.Count > 1)
                {
                    walkPath = findPathTask.Result;
                    // walkPath.RemoveAt(0);
                }

                findPathTask = null;
            }
        }

        private bool changePosition(GameTime gameTime, Vector2 destPos)
        {
            float newPosX = Position.X + Direction.X * Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float newPosY = Position.Y + Direction.Y * Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

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

        public void SetDirection(Vector2 newDirection)
        {
            if(this is Player)
            {
                Direction = newDirection;
            }
        }

        public override void Update(GameTime gameTime)
        {
            loadWalkpath(gameTime);

            if (walkPath != null)
            {
                var destPos = new Vector2(walkPath[0].x * WorldGrid.BlockSize.X + 16, walkPath[0].y * WorldGrid.BlockSize.Y + 20);

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
            else if(DestRealPosition != null)
            {
                if(Direction == Vector2.Zero)
                {
                    Direction = new Vector2(DestRealPosition.X - Position.X, DestRealPosition.Y - Position.Y);
                    Direction.Normalize();
                }

                bool couldWalk = changePosition(gameTime, DestRealPosition.ToVector());

                // Check if we couldn't move to position
                if (!couldWalk)
                {
                    WalkToBlock(DestRealPosition.ToBlockPosition());
                }
                else if (Math.Abs(DestRealPosition.X - Position.X) < GeometryUtils.SmallFloat && Math.Abs(DestRealPosition.Y - Position.Y) < GeometryUtils.SmallFloat)
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
            else if(this is Player)
            {
                if (Direction != Vector2.Zero)
                {
                    float newPosX = Position.X + Direction.X * Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    float newPosY = Position.Y + Direction.Y * Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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
