using Microsoft.Xna.Framework;

namespace Simulation.Game.Base
{
    public abstract class DrawableObject
    {
        public Vector2 position
        {
            get; private set;
        }

        public string ID
        {
            get; private set;
        }

        public DrawableObject(Vector2 position)
        {
            this.position = position;
            ID = Util.Util.getUUID();
        }

        public virtual void updatePosition(Vector2 newPosition)
        {
            position = newPosition;
        }
    }
}
