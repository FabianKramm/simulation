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
        public JObject CustomProperties;

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

        public virtual void UpdatePosition(WorldPosition newPosition)
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
            if (CustomProperties == null)
            {
                return default(T);
            }

            return SerializationUtils.GetFromObject<T>(CustomProperties, key);
        }

        public T GetOrAddCustomProperty<T>(string key, object value)
        {
            if (CustomProperties == null)
            {
                CustomProperties = new JObject();
            }

            if (CustomProperties.GetValue(key) == null)
            {
                SerializationUtils.AddToObject(CustomProperties, key, value);
            }

            return SerializationUtils.GetFromObject<T>(CustomProperties, key);
        }

        public void SetCustomProperty(string key, object value)
        {
            if (CustomProperties == null)
            {
                CustomProperties = new JObject();
            }

            SerializationUtils.AddToObject(CustomProperties, key, value);
        }

        public virtual void Update(GameTime gameTime)
        {
            CustomController?.Update(gameTime);
        }

        public abstract void ConnectToWorld();
        public abstract void DisconnectFromWorld();
    }
}
