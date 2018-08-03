using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using System;
using Simulation.Game.MetaData;

namespace Simulation.Game.Renderer.Entities
{
    public class LivingEntityRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, LivingEntity livingEntity)
        {
            if (livingEntity.RendererInformation == null)
                livingEntity.RendererInformation = LivingEntityType.CreateRendererInformation(livingEntity);

            if (livingEntity is MovingEntity)
            {
                MovingEntityRenderer.Draw(spriteBatch, gameTime, (MovingEntity)livingEntity);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
