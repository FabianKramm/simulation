using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class GameObjectSerializer
    {
        private static readonly Type drawableObjectType = typeof(GameObject);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(drawableObjectType);

        protected static void Deserialize(ref JObject jObject, GameObject gameObject)
        {
            SerializationUtils.SetFromObject(jObject, gameObject, drawableObjectType, serializeableProperties);
        }

        protected static void Serialize(GameObject gameObject, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, gameObject, drawableObjectType, serializeableProperties);
        }
    }
}
