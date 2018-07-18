using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Simulation.Game.Serialization;
using Simulation.Game.Serialization.Objects;
using Simulation.Game.World;
using Simulation.Util;
using System;
using System.IO;

namespace Simulation.Game.Generator
{
    public class WorldLoader
    {
        private static NamedLock<string> fileLocks = new NamedLock<string>();

        public static bool DoesWorldGridChunkExist(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                if (!File.Exists(chunkPath))
                {
                    return false;
                }

                return true;
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static void SaveInterior(Interior interior)
        {
            var chunkPath = Path.Combine(Util.Util.GetInteriorSavePath(), interior.ID);

            fileLocks.Enter(chunkPath);

            try
            {
                using (var stream = File.OpenWrite(chunkPath))
                using (var writer = new BsonWriter(stream))
                {
                    InteriorSerializer.Serialize(interior).WriteTo(writer);
                }
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static Interior LoadInterior(string ID)
        {
            if (ID == Interior.Outside) throw new Exception("Cannot load outside interior!");

            var chunkPath = Path.Combine(Util.Util.GetInteriorSavePath(), ID);

            if (!File.Exists(chunkPath))
            {
                throw new Exception("Cannot find interior with ID " + ID);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                Interior interior;

                using (var stream = File.OpenRead(chunkPath))
                using (var reader = new BsonReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    interior = InteriorSerializer.Deserialize((JObject)jToken);
                }

                return interior;
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static void SaveWalkableGridChunk(int chunkX, int chunkY, WalkableGridChunk chunk)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                byte[] bytes;

                chunk.copyDataTo(out bytes);
                File.WriteAllBytes(chunkPath, bytes);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static WalkableGridChunk LoadWalkableGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.WorldGenerator.generateChunk(chunkX * WalkableGrid.WalkableGridBlockChunkSize.X, chunkY * WalkableGrid.WalkableGridBlockChunkSize.Y);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                var content = File.ReadAllBytes(chunkPath);

                return WalkableGridChunk.createChunkFrom(chunkX, chunkY, ref content);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static WorldGridChunk LoadWorldGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.WorldGenerator.generateChunk(chunkX * WorldGrid.WorldChunkBlockSize.X, chunkY * WorldGrid.WorldChunkBlockSize.Y);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                WorldGridChunk worldGridChunk;

                using (var stream = File.OpenRead(chunkPath))
                using (var reader = new BsonReader(stream))
                {
                    JToken jToken = JToken.ReadFrom(reader);

                    worldGridChunk = WorldGridChunkSerializer.Deserialize((JObject)jToken);
                }

                return worldGridChunk;
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static void SaveWorldGridChunk(int chunkX, int chunkY, WorldGridChunk chunk)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                using (var stream = File.OpenWrite(chunkPath))
                using (var writer = new BsonWriter(stream))
                {
                    WorldGridChunkSerializer.Serialize(chunk).WriteTo(writer);
                }
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }
    }
}
