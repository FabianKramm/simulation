using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using System;
using System.Collections.Generic;
using Simulation.Spritesheet;

namespace Simulation.Game.Renderer.Entities
{
    public class LivingEntityRenderer
    {
        private static Dictionary<LivingEntityType, Func<LivingEntityRendererInformation>> informationGeneratorLookup = new Dictionary<LivingEntityType, Func<LivingEntityRendererInformation>> {
            { LivingEntityType.NO_ENTITY, null },
            { LivingEntityType.PLAYER, () => LivingEntityRendererInformationFactory.createPlayerRenderInformation() },
            { LivingEntityType.GERALT, () => LivingEntityRendererInformationFactory.CreateGeraltRenderInformation() }
        };

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, LivingEntity livingEntity)
        {
            if(SimulationGame.VisibleArea.Contains(livingEntity.Position) && livingEntity.InteriorID == SimulationGame.Player.InteriorID)
            {
                if (livingEntity.RendererInformation == null)
                    livingEntity.RendererInformation = informationGeneratorLookup[livingEntity.LivingEntityType]();

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
