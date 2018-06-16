using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    public abstract class DrawableObject
    {
        private Vector2 _position;
        public Vector2 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                onPositionChange();
            }
        }

        public DrawableObject(Vector2 position)
        {
            this.position = position;
        }

        protected abstract void onPositionChange();
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
