using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public class StaticBlockingObject: HitableObject
    {
        private string texture;
        private Rectangle spriteRectangle;
        private Vector2 origin;

        public StaticBlockingObject(string texture, Rectangle spriteRectangle, Vector2 position, Rectangle relativeBlockingRectangle):
            base(position, relativeBlockingRectangle, World.BlockingType.BLOCKING)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;

            origin = new Vector2(0, spriteRectangle.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(texture), position, spriteRectangle, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, GeometryUtils.getLayerDepthFromYPosition(position.Y));

            base.Draw(spriteBatch);
        }
    }
}
