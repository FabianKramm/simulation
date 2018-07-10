using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Hud;
using Simulation.Spritesheet;
using Simulation.Util;
using System;
using Simulation.Util.Geometry;
using Simulation.Game.World;

namespace Simulation.Game.Effects
{
    public class Fireball: Effect
    {
        private static float maxDistance = 16 * WorldGrid.BlockSize.X;

        public float Angle;
        public bool HasHitTarget = false;

        private Vector2 startPosition;
        private float velocity = 0.4f;
        private Vector2 direction;

        public Fireball(LivingEntity origin, Vector2 target, Vector2? relativeOriginPosition = null) : base(origin.Position, origin, origin.InteriorID)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Vector2 newPosition = Vector2.Add(origin.Position, _relativeOriginPosition);

            direction = new Vector2(target.X - newPosition.X, target.Y - newPosition.Y);
            direction.Normalize();

            newPosition.X += (direction.X * WorldGrid.BlockSize.X);
            newPosition.Y += (direction.Y * WorldGrid.BlockSize.Y);

            startPosition = newPosition;

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            updatePosition(newPosition);
        }

        public override void Update(GameTime gameTime)
        {
            if(!HasHitTarget)
            {
                updatePosition(new Vector2(Position.X + (direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds), Position.Y + (direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds)));

                if (GeometryUtils.VectorsWithinDistance(Position.X, Position.Y, startPosition.X, startPosition.Y, maxDistance))
                {
                    var rotateVector = new Vector2(Position.X, Position.Y + 7.5f);
                    var rotatedPoint = GeometryUtils.Rotate(Angle, Position, ref rotateVector);
                    var collisionRect = new Rect((int)(rotatedPoint.X - 7.5f), (int)(rotatedPoint.Y - 7.5f), 15, 15);
                    var hittedObjects = CollisionUtils.GetHittedObjects(collisionRect, Origin, InteriorID);

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
