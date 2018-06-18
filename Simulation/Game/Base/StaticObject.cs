using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public class StaticObject: DrawableObject
    {
        private string texture;
        private Rectangle spriteRectangle;
        private Vector2 origin;
        private bool hasDepth;

        public StaticObject(string texture, Rectangle spriteRectangle, Vector2 position, bool hasDepth = false) :
            base(position)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;
            this.hasDepth = hasDepth;

            origin = new Vector2(0, spriteRectangle.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(texture), position, spriteRectangle, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, hasDepth ? GeometryUtils.getLayerDepthFromYPosition(position.Y) : GeometryUtils.getLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
        }
    }
}
