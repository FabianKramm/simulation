using Newtonsoft.Json.Linq;
using Simulation.Game.World;
using Simulation.Util;
using System;

namespace Simulation.Game.Serialization
{
    public class WorldGridChunkSerializer: WorldPartSerialization
    {
        private static readonly Type worldGridChunkType = typeof(WorldGridChunk);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(worldGridChunkType);

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
            WorldPartSerialization.Deserialize(ref jObject, worldGridChunk);

            SerializationUtils.SetFromObject(jObject, worldGridChunk, worldGridChunkType, serializeableProperties);
        }

        protected static void Serialize(WorldGridChunk worldGridChunk, ref JObject jObject)
        {
            WorldPartSerialization.Serialize(worldGridChunk, ref jObject);

            SerializationUtils.AddToObject(jObject, worldGridChunk, worldGridChunkType, serializeableProperties);
        }
    }
}
