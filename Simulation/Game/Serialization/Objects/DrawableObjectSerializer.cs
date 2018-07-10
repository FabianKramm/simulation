using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class DrawableObjectSerializer
    {
        private static readonly Type drawableObjectType = typeof(GameObject);
        private static readonly string[] serializeableProperties = new string[] { "ID", "InteriorID", "Position", "BlockPosition" };

        protected static void Deserialize(ref JObject jObject, GameObject drawableObject)
        {
            SerializationUtils.SetFromObject(jObject, drawableObject, drawableObjectType, serializeableProperties);
        }

        protected static void Serialize(GameObject drawableObject, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, drawableObject, drawableObjectType, serializeableProperties);
        }
    }
}
