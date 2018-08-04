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
        public GameObjectRenderer CustomRenderer;
        public GameObjectController CustomController;

        [Serialize]
        private JObject customInformation;

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

            CustomController?.Init(this);
            CustomRenderer?.Init(this);
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

        public T GetCustomProperty<T>(string key)
        {
            if (customInformation == null)
            {
                return default(T);
            }

            return SerializationUtils.GetFromObject<T>(customInformation, key);
        }

        public T GetOrAddCustomProperty<T>(string key, object value)
        {
            if (customInformation == null)
            {
                customInformation = new JObject();
            }

            if (customInformation.GetValue(key) == null)
            {
                SerializationUtils.AddToObject(customInformation, key, value);
            }

            return SerializationUtils.GetFromObject<T>(customInformation, key);
        }

        public void SetCustomProperty(string key, object value)
        {
            if (customInformation == null)
            {
                customInformation = new JObject();
            }

            SerializationUtils.AddToObject(customInformation, key, value);
        }

        public virtual void Update(GameTime gameTime)
        {
            CustomController?.Update(gameTime);
        }
    }
}
