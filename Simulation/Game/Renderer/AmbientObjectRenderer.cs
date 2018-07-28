using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Util.Geometry;

namespace Simulation.Game.Renderer
{
    public class AmbientObjectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, AmbientObject ambientObject)
        {
            if(SimulationGame.VisibleArea.Contains(ambientObject.Position) && ambientObject.InteriorID == SimulationGame.Player.InteriorID)
            {
                var ambientObjectType = AmbientObjectType.lookup[ambientObject.AmbientObjectType];

                if (ambientObjectType != null)
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath), ambientObject.Position.ToVector(), new Rectangle(ambientObjectType.SpritePositions[0], ambientObjectType.SpriteBounds), GameRenderer.BlendColor, 0.0f, ambientObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, ambientObjectType.HasDepth ? GeometryUtils.GetLayerDepthFromPosition(ambientObject.Position.X, ambientObject.Position.Y) : GeometryUtils.GetLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
                }
            }
        }
    }
}
