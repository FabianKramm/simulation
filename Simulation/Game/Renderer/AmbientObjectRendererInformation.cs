using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Renderer
{
    public class AmbientObjectRenderInformation
    {
        public string texture
        {
            get; private set;
        }

        public Rectangle spriteRectangle
        {
            get; private set;
        }

        public Vector2 origin
        {
            get; private set;
        }

        public bool hasDepth
        {
            get; private set;
        }

        public AmbientObjectRenderInformation(string texture, Rectangle spriteRectangle, bool hasDepth = false)
        {
            this.texture = texture;
            this.spriteRectangle = spriteRectangle;
            this.hasDepth = hasDepth;

            origin = new Vector2(0, spriteRectangle.Height);
        }
    }
}
