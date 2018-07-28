using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;

namespace Simulation.Game.Effects
{
    public class Slash: Effect
    {
        public float Angle;
        public bool Flipped;
        public TimeSpan Duration = TimeSpan.FromMilliseconds(300);

        public Circle hitboxCircle;

        public Slash(MovingEntity origin, Vector2 target, float damage, bool flipped, Vector2? relativeOriginPosition = null) : base(origin.Position, origin)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Vector2 newPosition = Vector2.Add(origin.Position.ToVector(), _relativeOriginPosition);

            Vector2 direction = new Vector2(target.X - newPosition.X, target.Y - newPosition.Y);
            direction.Normalize();

            // Get Hitted Targets
            hitboxCircle = new Circle((int)(newPosition.X + direction.X * WorldGrid.BlockSize.X), (int)(newPosition.Y + direction.Y * WorldGrid.BlockSize.Y), WorldGrid.BlockSize.X);

            newPosition.X += (direction.X * WorldGrid.BlockSize.X * 1.5f);
            newPosition.Y += (direction.Y * WorldGrid.BlockSize.Y * 1.5f);

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            Flipped = flipped;

            origin.CanWalk = false;

            updatePosition(new WorldPosition(newPosition, origin.InteriorID));

            List<LivingEntity> hittedObjects = CollisionUtils.GetLivingHittedObjects(hitboxCircle, origin.InteriorID, origin);

            foreach (var hittedObject in hittedObjects)
                if (hittedObject is LivingEntity)
                    ((LivingEntity)hittedObject).ModifyHealth((int)-damage);
        }

        public override void Update(GameTime gameTime)
        {
            Duration -= gameTime.ElapsedGameTime;

            if(Duration.TotalMilliseconds <= 0)
            {
                ((MovingEntity)Origin).CanWalk = true;
                IsFinished = true;
            }
        }
    }
}
