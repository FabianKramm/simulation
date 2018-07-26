using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;
using Simulation.Util.Geometry;
using Simulation.Game.World;
using Simulation.Util.Collision;

namespace Simulation.Game.Effects
{
    public class Fireball: Effect
    {
        public static readonly float MaxDistance = 16 * WorldGrid.BlockSize.X;

        public float Angle;
        public bool HasHitTarget = false;

        private Vector2 startPosition;
        private Vector2 direction;

        private float velocity = 0.4f;

        public Fireball(LivingEntity origin, Vector2 target, Vector2? relativeOriginPosition = null) : base(origin.Position, origin)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Vector2 newPosition = Vector2.Add(origin.Position.ToVector(), _relativeOriginPosition);

            direction = new Vector2(target.X - newPosition.X, target.Y - newPosition.Y);
            direction.Normalize();

            newPosition.X += (direction.X * WorldGrid.BlockSize.X);
            newPosition.Y += (direction.Y * WorldGrid.BlockSize.Y);

            startPosition = newPosition;

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            updatePosition(new WorldPosition(newPosition, origin.InteriorID));
        }

        public override void Update(GameTime gameTime)
        {
            if(!HasHitTarget)
            {
                updatePosition(new WorldPosition(Position.X + (direction.X * velocity * (int)gameTime.ElapsedGameTime.TotalMilliseconds), Position.Y + (direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds), Position.InteriorID));

                if (GeometryUtils.VectorsWithinDistance(Position.X, Position.Y, startPosition.X, startPosition.Y, MaxDistance))
                {
                    var rotateVector = new Vector2(Position.X, Position.Y + 7.5f);
                    var rotatedPoint = GeometryUtils.Rotate(Angle, Position.ToVector(), ref rotateVector);
                    var collisionRect = new Rect((int)(rotatedPoint.X - 7.5f), (int)(rotatedPoint.Y - 7.5f), 15, 15);
                    var hittedObjects = CollisionUtils.GetHittedObjects(collisionRect, Position.InteriorID, Origin);

                    if(hittedObjects.Count == 0)
                    {
                        if (CollisionUtils.IsHitableBlockHitted(collisionRect, InteriorID))
                        {
                            HasHitTarget = true;
                        }
                    }
                    else
                    {
                        HasHitTarget = true;

                        // TODO: Apply effect on targets
                    }
                }
                else
                {
                    IsFinished = true;
                }
            }
            else
            {
                if(InteriorID != SimulationGame.Player.InteriorID || effectRendererInformation == null || effectRendererInformation.IsFinished)
                {
                    IsFinished = true;
                }
            }
        }
    }
}
