using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Renderer.Effects;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;

namespace Simulation.Game.Effects
{
    public class Slash: Effect
    {
        public float Angle;
        public Vector2 Position;
        public bool Flipped;
        public TimeSpan Duration = TimeSpan.FromMilliseconds(300);

        public Slash(MovingEntity origin, Vector2 target, bool flipped, Vector2? relativeOriginPosition = null) : base(origin)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Position = Vector2.Add(origin.Position, _relativeOriginPosition);

            Vector2 direction = new Vector2(target.X - Position.X, target.Y - Position.Y);
            direction.Normalize();

            Position.X += (direction.X * WorldGrid.BlockSize.X);
            Position.Y += (direction.Y * WorldGrid.BlockSize.Y);

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            Flipped = flipped;

            origin.CanWalk = false;
        }

        public override void Update(GameTime gameTime)
        {
            Duration -= gameTime.ElapsedGameTime;

            if(Duration.Milliseconds <= 0)
            {
                ((MovingEntity)origin).CanWalk = true;
                IsFinished = true;
            }
        }
    }
}
