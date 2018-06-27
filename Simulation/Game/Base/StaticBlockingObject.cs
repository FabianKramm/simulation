using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Renderer;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public class StaticBlockingObject: HitableObject
    {
        public StaticBlockingObject(InteractiveObjectType interactiveObjectType, Vector2 position, Rectangle relativeBlockingRectangle):
            base(interactiveObjectType, position, relativeBlockingRectangle, World.BlockingType.BLOCKING) {}
    }
}
