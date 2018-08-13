using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;

namespace Simulation.Game.Renderer
{
    public class AmbientObjectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, AmbientObject ambientObject)
        {
            var ambientObjectType = AmbientObjectType.lookup[ambientObject.AmbientObjectType];

            if (ambientObjectType != null)
            {
                if(ambientObjectType.SpritePositions.Length > 1)
                {
                    if (ambientObject.ObjectAnimation == null)
                    {
                        ambientObject.ObjectAnimation = AmbientObjectType.CreateAnimation(ambientObject);
                        ambientObject.ObjectAnimation.Start(Repeat.Mode.Loop);
                    }

                    ambientObject.ObjectAnimation.Update(gameTime);
                    spriteBatch.Draw(ambientObject.ObjectAnimation, ambientObject.Position.ToVector(), color: GameRenderer.BlendColor, layerDepth: ambientObjectType.HasDepth ? GeometryUtils.GetLayerDepthFromPosition(ambientObject.Position.X, ambientObject.Position.Y) : GeometryUtils.GetLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
                }
                else
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath), ambientObject.Position.ToVector(), new Rectangle(ambientObjectType.SpritePositions[0], ambientObjectType.SpriteBounds), GameRenderer.BlendColor, 0.0f, ambientObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, ambientObjectType.HasDepth ? GeometryUtils.GetLayerDepthFromPosition(ambientObject.Position.X, ambientObject.Position.Y) : GeometryUtils.GetLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
                }
            }
        }
    }
}
