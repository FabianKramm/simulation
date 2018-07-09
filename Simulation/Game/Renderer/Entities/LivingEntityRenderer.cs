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
            if(SimulationGame.VisibleArea.Contains(livingEntity.Position) && livingEntity.InteriorID == SimulationGame.Player.InteriorID)
            {
                if (livingEntity.RendererInformation == null)
                {
                    livingEntity.RendererInformation = informationGeneratorLookup[livingEntity.LivingEntityType]();
                }

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
}
