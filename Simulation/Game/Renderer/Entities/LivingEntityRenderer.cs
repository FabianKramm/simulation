using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base.Entity;
using Simulation.Util;
using System;
using System.Collections.Generic;
using Simulation.Spritesheet;

namespace Simulation.Game.Renderer.Entities
{
    public enum LivingEntityType
    {
        NO_ENTITY = 0,
        PLAYER,
    }

    public class LivingEntityRenderer
    {
        private static Dictionary<LivingEntityType, Func<LivingEntityRendererInformation>> informationGeneratorLookup = new Dictionary<LivingEntityType, Func<LivingEntityRendererInformation>> {
            { LivingEntityType.NO_ENTITY, null },
            { LivingEntityType.PLAYER, () => LivingEntityRendererInformationFactory.createPlayerRenderInformation() }
        };

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, LivingEntity livingEntity)
        {
            if(livingEntity.rendererInformation == null)
            {
                livingEntity.rendererInformation = informationGeneratorLookup[livingEntity.livingEntityType]();
            }

            if(livingEntity is MovingEntity)
            {
                MovingEntity movingEntity = (MovingEntity)livingEntity;
                WalkingDirection newWalkingDirection = Movement.getWalkingDirectionFromVector(movingEntity.direction);

                movingEntity.rendererInformation.Update(gameTime, newWalkingDirection);

                spriteBatch.Draw(movingEntity.rendererInformation.currentAnimation, movingEntity.position, layerDepth: GeometryUtils.getLayerDepthFromPosition(movingEntity.position.X, movingEntity.position.Y));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
