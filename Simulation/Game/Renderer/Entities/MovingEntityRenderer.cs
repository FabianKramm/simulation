using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;

namespace Simulation.Game.Renderer.Entities
{
    class MovingEntityRenderer
    {
        private static SpriteFont font;
        private static Texture2D healthTexture;

        public static void LoadContent()
        {
            font = SimulationGame.ContentManager.Load<SpriteFont>("ArialSmall");

            healthTexture = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            healthTexture.SetData<Color>(new Color[] { Color.White });
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, MovingEntity movingEntity)
        {
            var livingEntityType = LivingEntityType.lookup[movingEntity.LivingEntityType];
            WalkingDirection newWalkingDirection = MovementUtils.GetWalkingDirectionFromVector(movingEntity.Direction);

            movingEntity.RendererInformation.Update(gameTime, newWalkingDirection);

            if(!movingEntity.IsWalking)
            {
                movingEntity.RendererInformation.currentAnimation.Reset();
            }

            var adjustedYPosition = movingEntity.Position.Y + movingEntity.YPositionDepthOffset;

            if(movingEntity.IsDead())
            {
                movingEntity.RendererInformation.Update(gameTime, WalkingDirection.Left);
                movingEntity.RendererInformation.currentAnimation.Reset();

                spriteBatch.Draw(movingEntity.RendererInformation.currentAnimation, new Vector2((int)movingEntity.Position.X, (int)movingEntity.Position.Y), rotation: MathHelper.Pi / 2, color: GameRenderer.BlendColor, layerDepth: GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X, adjustedYPosition));
            }
            else
            {
                spriteBatch.Draw(movingEntity.RendererInformation.currentAnimation, new Vector2((int)movingEntity.Position.X, (int)movingEntity.Position.Y), color: GameRenderer.BlendColor, layerDepth: GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X, adjustedYPosition));
            }

            // Draw Speech Bubble
            if (movingEntity.RendererInformation.SpeechLine != null)
            {
                var depth = GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X, adjustedYPosition);
                var depth2 = GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X + 1, adjustedYPosition);

                var bubbleYOffset = (int)movingEntity.Position.Y - livingEntityType.SpriteBounds.Y - 20;

                var stringWidth = font.MeasureString(movingEntity.RendererInformation.SpeechLine).X;
                var bubbleWidth = stringWidth - 8;
                var bubbleStartPos = new Vector2((int)movingEntity.Position.X - 10 - bubbleWidth, bubbleYOffset);
                var bubbleContentPos = new Vector2((int)movingEntity.Position.X - bubbleWidth, bubbleYOffset);
                var bubbleEndPos = new Vector2((int)movingEntity.Position.X - 10, bubbleYOffset);

                // Draw bubble start
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(@"GUI\SpeechBubble"), bubbleStartPos, new Rectangle(0, 0, 10, 27), GameRenderer.BlendColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);

                // Draw bubble content
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(@"GUI\SpeechBubble"), bubbleContentPos, new Rectangle(10, 0, 20, 27), GameRenderer.BlendColor, 0.0f, Vector2.Zero, new Vector2((stringWidth-18)/20.0f, 1.0f), SpriteEffects.None, depth);

                // Draw string
                spriteBatch.DrawString(font, movingEntity.RendererInformation.SpeechLine, new Vector2(bubbleContentPos.X, bubbleYOffset + 5), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth2);

                // Draw bubble end
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(@"GUI\SpeechBubble"), bubbleEndPos, new Rectangle(110, 0, 27, 27), GameRenderer.BlendColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
            }

            // Draw Health
            if(movingEntity.CurrentLife < movingEntity.MaximumLife)
            {
                spriteBatch.Draw(healthTexture, new Vector2(movingEntity.Position.X - livingEntityType.SpriteBounds.X / 2, movingEntity.Position.Y - livingEntityType.SpriteBounds.Y), 
                    new Rectangle(0, 0, 1, 1), Color.Red, 0.0f, Vector2.Zero, new Vector2(((float)movingEntity.CurrentLife / (float)movingEntity.MaximumLife) * livingEntityType.SpriteBounds.X, 2.0f), SpriteEffects.None, GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X, adjustedYPosition));
            }

            if (SimulationGame.IsDebug)
            {
                if (movingEntity.IsBlocking())
                {
                    SimulationGame.PrimitiveDrawer.Rectangle(movingEntity.UnionBounds.ToXnaRectangle(), Color.Red);
                }
                else if (movingEntity.IsHitable())
                {
                    SimulationGame.PrimitiveDrawer.Rectangle(movingEntity.HitBoxBounds.ToXnaRectangle(), Color.White);
                    SimulationGame.PrimitiveDrawer.Rectangle(movingEntity.BlockingBounds.ToXnaRectangle(), Color.Red);
                }
            }
        }
    }
}
