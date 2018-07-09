using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public abstract class DrawableObject
    {
        public string InteriorID = null;

        public Vector2 Position
        {
            get; private set;
        }

        public Point BlockPosition
        {
            get; private set;
        }

        public string ID
        {
            get; private set;
        }

        public bool IsDestroyed
        {
            get; private set;
        }

        // Create from JSON
        protected DrawableObject() {}

        protected DrawableObject(Vector2 position)
        {
            this.Position = position;
            ID = Util.Util.getUUID();
        }

        public virtual void UpdatePosition(Vector2 newPosition)
        {
            Position = new Vector2((int)newPosition.X, (int)newPosition.Y);
            BlockPosition = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
