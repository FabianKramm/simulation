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
                var depth = GeometryUtils.GetLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration);

                if (ambientObjectType.InForeground == true)
                {
                    depth = 1.0f;
                }
                else if(ambientObjectType.HasDepth == true)
                {
                    depth = GeometryUtils.GetLayerDepthFromPosition(ambientObject.Position.X, ambientObject.Position.Y + ambientObject.YPositionDepthOffset);
                }

                if (ambientObjectType.SpritePositions.Length > 1)
                {
                    if (ambientObject.ObjectAnimation == null)
                    {
                        ambientObject.ObjectAnimation = AmbientObjectType.CreateAnimation(ambientObject);
                        ambientObject.ObjectAnimation.Start(Repeat.Mode.Loop);
                    }

                    ambientObject.ObjectAnimation.Update(gameTime);
                    spriteBatch.Draw(ambientObject.ObjectAnimation, ambientObject.Position.ToVector(), color: GameRenderer.BlendColor, layerDepth: depth);
                }
                else
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath), ambientObject.Position.ToVector(), new Rectangle(ambientObjectType.SpritePositions[0], ambientObjectType.SpriteBounds), GameRenderer.BlendColor, 0.0f, ambientObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, depth);
                }
            }
        }
    }
}
