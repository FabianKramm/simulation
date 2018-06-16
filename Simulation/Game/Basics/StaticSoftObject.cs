using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    public class StaticSoftObject: DrawableObject
    {
        private string texture;
        private Rectangle spriteRectangle;
        private Vector2 origin;

        public StaticSoftObject(string texture, Rectangle spriteRectangle, Vector2 position) :
            base(position)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;

            origin = new Vector2(0, spriteRectangle.Height);
        }

        protected override void onPositionChange() {}

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(texture), position, spriteRectangle, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, GeometryUtils.getLayerDepthFromYPosition(position.Y));
        }
    }
}
