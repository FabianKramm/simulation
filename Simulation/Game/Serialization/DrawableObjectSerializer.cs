using Newtonsoft.Json.Linq;
using Simulation.Game.Base;
using System;

namespace Simulation.Game.Serialization
{
    public class DrawableObjectSerializer
    {
        private static readonly Type drawableObjectType = typeof(DrawableObject);
        private static readonly string[] serializeableProperties = new string[] { "ID", "position" };

        protected static void Deserialize(ref JObject jObject, DrawableObject drawableObject)
        {
            SerializationUtils.SetFromObject(jObject, drawableObject, drawableObjectType, serializeableProperties);
        }

        protected static void Serialize(DrawableObject drawableObject, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, drawableObject, drawableObjectType, serializeableProperties);
        }
    }
}
