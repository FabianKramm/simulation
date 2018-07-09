﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Effects;
using Simulation.Spritesheet;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Renderer.Effects
{
    public class SlashRendererInformation: EffectRendererInformation
    {
        public Animation slashAnimation;
        public float Angle;

        public SlashRendererInformation(float angle, bool flipped)
        {
            var texture = SimulationGame.ContentManager.Load<Texture2D>(@"Spells\Slash\Slash");
            var sheet = new Spritesheet.Spritesheet(texture).WithFrameEffect(flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None).WithGrid((64, 64)).WithCellOrigin(new Point(32, 64)).WithFrameDuration(70);

            slashAnimation = sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0), (4, 0));
            slashAnimation.Start(Repeat.Mode.Once);

            Angle = flipped ? angle - 0.5f * (float)Math.PI : angle;
        }
    }

    public class SlashRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, Slash slash)
        {
            SlashRendererInformation slashRendererInformation = (SlashRendererInformation)slash.effectRendererInformation;

            slashRendererInformation.slashAnimation.Update(gameTime);

            if(slashRendererInformation.slashAnimation.IsStarted)
            {
                if (SimulationGame.VisibleArea.Contains(slash.Position))
                    spriteBatch.Draw(slashRendererInformation.slashAnimation, slash.Position, rotation: slashRendererInformation.Angle, layerDepth: 1.0f);
            }
            else
            {
                slashRendererInformation.IsFinished = true;
            }
        }
    }
}
