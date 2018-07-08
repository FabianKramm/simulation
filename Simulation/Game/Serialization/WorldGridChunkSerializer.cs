﻿using Newtonsoft.Json.Linq;
using Simulation.Game.Base;
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
            "realChunkBounds"
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
            JArray ambientObjects = new JArray(jObject.GetValue("ambientObjects"));

            foreach (var ambientObject in ambientObjects)
                worldGridChunk.ambientObjects.Add((AmbientObject)WorldObjectSerializer.Deserialize(new JObject(ambientObject)));

            // Deserialize Hitable Objects
            JArray containedObjects = new JArray(jObject.GetValue("containedObjects"));

            foreach (var containedObject in containedObjects)
                worldGridChunk.containedObjects.Add((HitableObject)WorldObjectSerializer.Deserialize(new JObject(containedObject)));
        }

        protected static void Serialize(WorldGridChunk worldGridChunk, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, worldGridChunk, worldGridChunkType, serializeableProperties);

            // Serialize Ambient Objects
            JArray ambientObjects = new JArray();

            foreach(var ambientObject in worldGridChunk.ambientObjects)
                ambientObjects.Add(WorldObjectSerializer.Serialize(ambientObject));

            jObject.Add("ambientObjects", ambientObjects);

            // Serialize Hitable Objects
            JArray containedObjects = new JArray();

            foreach (var containedObject in worldGridChunk.containedObjects)
                containedObjects.Add(WorldObjectSerializer.Serialize(containedObject));

            jObject.Add("containedObjects", containedObjects);
        }
    }
}
