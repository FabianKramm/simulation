using Newtonsoft.Json.Linq;
using Simulation.Game.Base;
using Simulation.Game.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization
{
    class WorldObjectSerializer
    {
        private static Dictionary<string, Func<JObject, DrawableObject>> worldObjectDeserializationLookup = new Dictionary<string, Func<JObject, DrawableObject>>()
        {
            {typeof(AmbientObject).FullName, (JObject jObject) => AmbientObjectSerializer.Deserialize(jObject)},
            {typeof(AmbientHitableObject).FullName, (JObject jObject) => AmbientHitableObjectSerializer.Deserialize(jObject)},
            {typeof(MovingEntity).FullName, (JObject jObject) => MovingEntitySerializer.Deserialize(jObject)},
            {typeof(DurableEntity).FullName, (JObject jObject) => DurableEntitySerializer.Deserialize(jObject)}
        };

        private static Dictionary<string, Func<DrawableObject, JObject>> worldObjectSerializationLookup = new Dictionary<string, Func<DrawableObject, JObject>>()
        {
            {typeof(AmbientObject).FullName, (DrawableObject drawableObject) => AmbientObjectSerializer.Serialize((AmbientObject)drawableObject)},
            {typeof(AmbientHitableObject).FullName, (DrawableObject drawableObject) => AmbientHitableObjectSerializer.Serialize((AmbientHitableObject)drawableObject)},
            {typeof(MovingEntity).FullName, (DrawableObject drawableObject) => MovingEntitySerializer.Serialize((MovingEntity)drawableObject)},
            {typeof(DurableEntity).FullName, (DrawableObject drawableObject) => DurableEntitySerializer.Serialize((DurableEntity)drawableObject)}
        };

        public static JObject Serialize(DrawableObject drawableObject) => worldObjectSerializationLookup[drawableObject.GetType().FullName](drawableObject);
        public static DrawableObject Deserialize(JObject jObject) => worldObjectDeserializationLookup[jObject.GetValue("type").ToString()](jObject);
    }
}
