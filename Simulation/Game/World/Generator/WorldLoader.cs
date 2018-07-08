using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Simulation.Game.Hud;
using Simulation.Game.Serialization;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Simulation.Game.World.Generator
{
    public class WorldLoader
    {
        private static NamedLock fileLocks = new NamedLock();

        public static bool doesWorldGridChunkExist(int chunkX, int chunkY)
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

        public static void saveWalkableGridChunk(int chunkX, int chunkY, WalkableGridChunk chunk)
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

        public static WalkableGridChunk loadWalkableGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.worldGenerator.generateChunk(chunkX * WalkableGrid.WalkableGridBlockChunkSize.X, chunkY * WalkableGrid.WalkableGridBlockChunkSize.Y);
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

        public static WorldGridChunk loadWorldGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.worldGenerator.generateChunk(chunkX * WorldGrid.WorldChunkBlockSize.X, chunkY * WorldGrid.WorldChunkBlockSize.Y);
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

        public static void saveWorldGridChunk(int chunkX, int chunkY, WorldGridChunk chunk)
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
