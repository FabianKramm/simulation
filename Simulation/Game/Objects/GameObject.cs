using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Interfaces;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Objects
{
    public abstract class GameObject
    {
        protected object positionChangeLock;

        public GameObjectRenderer CustomRenderer;
        public GameObjectController CustomController;

        [Serialize]
        public JObject CustomInformation;

        [Serialize]
        public string ID
        {
            get; private set;
        }

        [Serialize]
        public WorldPosition Position
        {
            get; private set;
        }

        [Serialize]
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

        public virtual void Update(GameTime gameTime)
        {
            CustomController?.Update(gameTime);
        }
    }
}
