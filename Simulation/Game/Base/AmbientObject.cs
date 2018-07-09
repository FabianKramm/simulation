using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Simulation.Game.Renderer;
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
        public AmbientObjectType AmbientObjectType
        {
            get; private set;
        }

        // Create from JSON
        protected AmbientObject() {}

        public AmbientObject(AmbientObjectType ambientObjectType, Vector2 position, bool hasDepth = false) :
            base(position)
        {
            this.AmbientObjectType = ambientObjectType;
        }
    }
}
