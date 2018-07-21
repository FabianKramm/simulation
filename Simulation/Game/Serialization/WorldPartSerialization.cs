using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization.Objects;
using Simulation.Game.World;
using System;

namespace Simulation.Game.Serialization
{
    public class WorldPartSerialization
    {
        private static readonly Type worldPartType = typeof(WorldPart);
        private static readonly string[] serializeableProperties = new string[] {
            "blockingGrid",
            "WorldLinks",
            "IsPersistent",
            "Dimensions"
        };

        protected static void Deserialize(ref JObject jObject, WorldPart worldPart)
        {
            SerializationUtils.SetFromObject(jObject, worldPart, worldPartType, serializeableProperties);

            // Deserialize Ambient Objects
            JArray ambientObjects = (JArray)jObject.GetValue("AmbientObjects");

            foreach (var ambientObject in ambientObjects)
                worldPart.AddAmbientObject((AmbientObject)WorldObjectSerializer.Deserialize((JObject)ambientObject));

            // Deserialize Hitable Objects
            JArray containedObjects = (JArray)jObject.GetValue("ContainedObjects");

            foreach (var containedObject in containedObjects)
                worldPart.AddContainedObject((HitableObject)WorldObjectSerializer.Deserialize((JObject)containedObject));
        }

        protected static void Serialize(WorldPart worldPart, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, worldPart, worldPartType, serializeableProperties);

            // Serialize Ambient Objects
            JArray ambientObjects = new JArray();

            if (worldPart.AmbientObjects != null)
                foreach (var ambientObject in worldPart.AmbientObjects)
                    ambientObjects.Add(WorldObjectSerializer.Serialize(ambientObject));

            jObject.Add("AmbientObjects", ambientObjects);

            // Serialize Hitable Objects
            JArray containedObjects = new JArray();

            if (worldPart.ContainedObjects != null)
                foreach (var containedObject in worldPart.ContainedObjects)
                {
                    if (containedObject is DurableEntity) continue;

                    containedObjects.Add(WorldObjectSerializer.Serialize(containedObject));
                }

            jObject.Add("ContainedObjects", containedObjects);
        }
    }
}
