using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization
{
    class WorldObjectSerializer
    {
        private static Dictionary<string, Func<JObject, GameObject>> worldObjectDeserializationLookup = new Dictionary<string, Func<JObject, GameObject>>()
        {
            {typeof(AmbientObject).FullName, (JObject jObject) => AmbientObjectSerializer.Deserialize(jObject)},
            {typeof(AmbientHitableObject).FullName, (JObject jObject) => AmbientHitableObjectSerializer.Deserialize(jObject)},
            {typeof(MovingEntity).FullName, (JObject jObject) => MovingEntitySerializer.Deserialize(jObject)},
            {typeof(DurableEntity).FullName, (JObject jObject) => DurableEntitySerializer.Deserialize(jObject)}
        };

        private static Dictionary<string, Func<GameObject, JObject>> worldObjectSerializationLookup = new Dictionary<string, Func<GameObject, JObject>>()
        {
            {typeof(AmbientObject).FullName, (GameObject drawableObject) => AmbientObjectSerializer.Serialize((AmbientObject)drawableObject)},
            {typeof(AmbientHitableObject).FullName, (GameObject drawableObject) => AmbientHitableObjectSerializer.Serialize((AmbientHitableObject)drawableObject)},
            {typeof(MovingEntity).FullName, (GameObject drawableObject) => MovingEntitySerializer.Serialize((MovingEntity)drawableObject)},
            {typeof(DurableEntity).FullName, (GameObject drawableObject) => DurableEntitySerializer.Serialize((DurableEntity)drawableObject)}
        };

        public static JObject Serialize(GameObject drawableObject) => worldObjectSerializationLookup[drawableObject.GetType().FullName](drawableObject);
        public static GameObject Deserialize(JObject jObject) => worldObjectDeserializationLookup[jObject.GetValue("type").ToString()](jObject);
    }
}
