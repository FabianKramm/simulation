using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.World;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    public class StaticObject: CollidableRectangleObject
    {
        private string texture;
        private Rectangle spriteRectangle;

        public StaticObject(string texture, Rectangle spriteRectangle, Vector2 position, Point upperLeftPointVector, Point collisionRectSize, CollisionType collisionType = CollisionType.SOLID_OBJECT):
            base(position, upperLeftPointVector, collisionRectSize, collisionType)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(texture), position, spriteRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GeometryUtils.getLayerDepthFromYPosition(position.Y + spriteRectangle.Height));

            base.Draw(spriteBatch);
        }
    }
}
