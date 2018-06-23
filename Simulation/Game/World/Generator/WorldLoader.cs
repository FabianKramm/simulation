using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.World.Generator
{
    public class WorldLoader
    {
        public static void saveWalkableGridChunk(int chunkX, int chunkY, WalkableGridChunk chunk)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "y" + Math.Abs(chunkY) : "" + chunkY));
            byte[] bytes;

            chunk.copyDataTo(out bytes);

            File.WriteAllBytes(chunkPath, bytes);
        }

        public static WalkableGridChunk loadWalkableGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "y" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.worldGenerator.generateChunk(chunkX * WalkableGrid.WalkableGridBlockChunkSize.X, chunkY * WalkableGrid.WalkableGridBlockChunkSize.Y);
            }

            var content = File.ReadAllBytes(chunkPath);

            return WalkableGridChunk.createChunkFrom(ref content);
        }

        public static WorldGridChunk loadWorldGridChunk(int chunkX, int chunkY)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "y" + Math.Abs(chunkY) : "" + chunkY));

            if (!File.Exists(chunkPath))
            {
                SimulationGame.worldGenerator.generateChunk(chunkX * World.WorldChunkBlockSize.X, chunkY * World.WorldChunkBlockSize.Y);
            }

            WorldGridChunk worldGridChunk;

            using (var stream = File.OpenRead(chunkPath))
            using (var reader = new BsonReader(stream))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                worldGridChunk = serializer.Deserialize<WorldGridChunk>(reader);
            }

            return worldGridChunk;
        }

        public static void saveWorldGridChunk(int chunkX, int chunkY, WorldGridChunk chunk)
        {
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "y" + Math.Abs(chunkY) : "" + chunkY));

            using (var stream = File.OpenWrite(chunkPath))
            using (var writer = new BsonWriter(stream))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                serializer.Serialize(writer, chunk);
            }
        }
    }
}
