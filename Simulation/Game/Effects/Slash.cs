using System;
using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.Effects
{
    public class Slash: Effect
    {
        public float Angle;
        public bool Flipped;
        public TimeSpan Duration = TimeSpan.FromMilliseconds(300);

        public Slash(MovingEntity origin, Vector2 target, bool flipped, Vector2? relativeOriginPosition = null) : base(origin.Position, origin, origin.InteriorID)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Vector2 newPosition = Vector2.Add(origin.Position, _relativeOriginPosition);

            Vector2 direction = new Vector2(target.X - newPosition.X, target.Y - newPosition.Y);
            direction.Normalize();

            newPosition.X += (direction.X * WorldGrid.BlockSize.X * 1.5f);
            newPosition.Y += (direction.Y * WorldGrid.BlockSize.Y * 1.5f);

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            Flipped = flipped;

            origin.CanWalk = false;

            updatePosition(newPosition);
        }

        public override void Update(GameTime gameTime)
        {
            Duration -= gameTime.ElapsedGameTime;

            if(Duration.Milliseconds <= 0)
            {
                ((MovingEntity)Origin).CanWalk = true;
                IsFinished = true;
            }
        }
    }
}
