﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base.Entity;
using Simulation.Game.Renderer.Effects;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Effects
{
    public class Slash: Effect
    {
        public float Angle;
        public Vector2 Position;

        public Slash(LivingEntity origin, Vector2 target, bool flipped, Vector2? relativeOriginPosition = null) : base(origin)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;
            Position = Vector2.Add(origin.Position, _relativeOriginPosition);

            Vector2 direction = new Vector2(target.X - Position.X, target.Y - Position.Y);
            direction.Normalize();

            Position.X += (direction.X * 12);
            Position.Y += (direction.Y * 12);

            Angle = GeometryUtils.GetAngleFromDirection(direction) + (float)Math.PI * 0.5f;

            effectRendererInformation = new SlashRendererInformation(Angle, flipped);
        }

        public override void Update(GameTime gameTime)
        {
            if(effectRendererInformation.IsFinished)
            {
                IsFinished = true;
            }
        }
    }
}
