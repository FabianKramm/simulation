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
        private float velocity = 0.3f;

        // Create from JSON
        protected MovingEntity() { }

        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds):
            base(livingEntityType, position, relativeHitBoxBounds) {}

        public void walkTo(int destBlockX, int destBlockY)
        {
            Point currentBlock = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, World.WorldGrid.BlockSize.X, World.WorldGrid.BlockSize.Y);

            findPathTask = PathFinder.FindPath(currentBlock.X, currentBlock.Y, destBlockX, destBlockY);

            walkPath = null;
            direction = Vector2.Zero;
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            if (canMove(newPosition))
            {
                SimulationGame.world.removeInteractiveObject(this);

                // TODO: Check if we are moving into unloaded area => if yes then we load the tile and unload us
                base.UpdatePosition(newPosition);

                SimulationGame.world.addInteractiveObject(this);
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
