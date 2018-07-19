using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects
{
    public abstract class GameObject
    {
        protected object positionChangeLock;

        public string ID
        {
            get; private set;
        }

        public WorldPosition Position
        {
            get; private set;
        }

        public Point BlockPosition
        {
            get; private set;
        }

        public bool IsDestroyed
        {
            get; private set;
        }

        public string InteriorID
        {
            get => Position.InteriorID;
        }

        // Create from JSON
        protected GameObject() {}

        protected GameObject(WorldPosition position)
        {
            Position = position.Clone();
            BlockPosition = Position.BlockPosition;

            ID = Util.Util.getUUID();
        }

        public virtual void UpdatePosition(WorldPosition newPosition)
        {
            Position = newPosition.Clone();
            BlockPosition = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
