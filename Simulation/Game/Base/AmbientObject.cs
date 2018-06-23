using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Simulation.Util;

/*
Requirements:
    - Travel through large area
    - Enable background actions for some npcs
 
 */
namespace Simulation.Game.Base
{
    public class AmbientObject: DrawableObject
    {
        [JsonProperty]
        private string texture;

        [JsonProperty]
        private Rectangle spriteRectangle;

        [JsonProperty]
        private Vector2 origin;

        [JsonProperty]
        private bool hasDepth;
        
        public AmbientObject(string texture, Rectangle spriteRectangle, Vector2 position, bool hasDepth = false) :
            base(position)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;
            this.hasDepth = hasDepth;

            origin = new Vector2(0, spriteRectangle.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(texture), position, spriteRectangle, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, hasDepth ? GeometryUtils.getLayerDepthFromPosition(position.X, position.Y) : GeometryUtils.getLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
        }
    }
}
