using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class WorldGridChunkSerializer
    {
        private static readonly Type worldGridChunkType = typeof(WorldGridChunk);
        private static readonly string[] serializeableProperties = new string[] {
            "blockingGrid",
            "RealChunkBounds",
            "WorldLinks"
        };

        public static WorldGridChunk Deserialize(JObject jObject)
        {
            WorldGridChunk worldGridChunk = ReflectionUtils.CallPrivateConstructor<WorldGridChunk>();

            Deserialize(ref jObject, worldGridChunk);

            return worldGridChunk;
        }

        public static JObject Serialize(WorldGridChunk worldGridChunk)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(worldGridChunkType, ref retObject);
            Serialize(worldGridChunk, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, WorldGridChunk worldGridChunk)
        {
            SerializationUtils.SetFromObject(jObject, worldGridChunk, worldGridChunkType, serializeableProperties);

            // Deserialize Ambient Objects
            JArray ambientObjects = (JArray)jObject.GetValue("AmbientObjects");

            foreach (var ambientObject in ambientObjects)
                worldGridChunk.AddAmbientObject((AmbientObject)WorldObjectSerializer.Deserialize((JObject)ambientObject));

            // Deserialize Hitable Objects
            JArray containedObjects = (JArray)jObject.GetValue("ContainedObjects");

            foreach (var containedObject in containedObjects)
                worldGridChunk.AddContainedObject((HitableObject)WorldObjectSerializer.Deserialize((JObject)containedObject));
        }

        protected static void Serialize(WorldGridChunk worldGridChunk, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, worldGridChunk, worldGridChunkType, serializeableProperties);

            // Serialize Ambient Objects
            JArray ambientObjects = new JArray();

            if (worldGridChunk.AmbientObjects != null)
                foreach (var ambientObject in worldGridChunk.AmbientObjects)
                    ambientObjects.Add(WorldObjectSerializer.Serialize(ambientObject));

            jObject.Add("AmbientObjects", ambientObjects);

            // Serialize Hitable Objects
            JArray containedObjects = new JArray();

            if (worldGridChunk.ContainedObjects != null)
                foreach (var containedObject in worldGridChunk.ContainedObjects)
                {
                    if (containedObject is DurableEntity) continue;

                    containedObjects.Add(WorldObjectSerializer.Serialize(containedObject));
                }

            jObject.Add("ContainedObjects", containedObjects);
        }
    }
}
