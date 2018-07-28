using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;

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

        // Json
        protected GameObject() { }

        public GameObject(WorldPosition realPosition)
        {
            ID = Util.Util.GetUUID();
            Position = realPosition.Clone();
        }

        public virtual void Init()
        {
            BlockPosition = Position.ToBlockPositionPoint();
        }

        protected virtual void UpdatePosition(WorldPosition newPosition)
        {
            ThreadingUtils.assertMainThread();

            Position = newPosition.Clone();
            BlockPosition = Position.ToBlockPositionPoint();
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
