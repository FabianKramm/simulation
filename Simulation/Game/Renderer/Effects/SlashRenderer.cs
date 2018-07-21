using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Effects;
using Simulation.Spritesheet;
using System;

namespace Simulation.Game.Renderer.Effects
{
    public class SlashRendererInformation: EffectRendererInformation
    {
        public Animation slashAnimation;
        public float Angle;

        public SlashRendererInformation(Slash slash)
        {
            var texture = SimulationGame.ContentManager.Load<Texture2D>(@"Spells\Slash\Slash");
            var sheet = new Spritesheet.Spritesheet(texture).WithFrameEffect(slash.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None).WithGrid((64, 64)).WithCellOrigin(new Point(32, 32)).WithFrameDuration(slash.Duration.Milliseconds / 5);

            slashAnimation = sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0), (4, 0));
            slashAnimation.Start(Repeat.Mode.Once);

            Angle = slash.Flipped ? slash.Angle - 0.5f * (float)Math.PI : slash.Angle;
        }
    }

    public class SlashRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, Slash slash)
        {
            if (slash.InteriorID == SimulationGame.Player.InteriorID && SimulationGame.VisibleArea.Contains(slash.Position))
            {
                if (slash.effectRendererInformation == null)
                    slash.effectRendererInformation = new SlashRendererInformation(slash);

                SlashRendererInformation slashRendererInformation = (SlashRendererInformation)slash.effectRendererInformation;

                slashRendererInformation.slashAnimation.Update(gameTime);

                spriteBatch.Draw(slashRendererInformation.slashAnimation, slash.Position.ToVector(), rotation: slashRendererInformation.Angle, layerDepth: 1.0f);

                if(SimulationGame.IsDebug)
                {
                    SimulationGame.PrimitiveDrawer.Circle(slash.hitboxCircle.ToVector(), slash.hitboxCircle.Radius, Color.Red);
                }
            }
        }
    }
}
