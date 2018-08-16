using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util;
using System;
using System.Diagnostics;
using System.IO;

namespace Simulation.Game.Generator
{
    public class WorldLoader
    {
        private static string persistentIdentifier = "persistent_";
        private static NamedLock<string> fileLocks = new NamedLock<string>();

        public static void ResetMetaData()
        {
            Util.Util.CreateGameFolders();

            if (File.Exists(Util.Util.GetBlockTypesSavePath()))
                File.Delete(Util.Util.GetBlockTypesSavePath());

            if (File.Exists(Util.Util.GetAmbientObjectTypesSavePath()))
                File.Delete(Util.Util.GetAmbientObjectTypesSavePath());

            if (File.Exists(Util.Util.GetAmbientHitableObjectTypesSavePath()))
                File.Delete(Util.Util.GetAmbientHitableObjectTypesSavePath());

            if (File.Exists(Util.Util.GetLivingEntityTypesSavePath()))
                File.Delete(Util.Util.GetLivingEntityTypesSavePath());

            if (File.Exists(Util.Util.GetBiomeTypesSavePath()))
                File.Delete(Util.Util.GetBiomeTypesSavePath());

            if (File.Exists(Util.Util.GetPointOfInterestSavePath()))
                File.Delete(Util.Util.GetPointOfInterestSavePath());
        }

        public static void ResetWorld()
        {
            Debug.Assert(SimulationGame.World == null, "Should only be called at startup!");

            Util.Util.CreateGameFolders();
            var gameFolder = Util.Util.GetGameFolder();

            string[] files = Directory.GetFiles(Util.Util.GetWalkableGridSavePath());

            foreach(var file in files)
                if (Path.GetFileName(file).StartsWith(persistentIdentifier) == false)
                    File.Delete(file);

            files = Directory.GetFiles(Util.Util.GetWorldSavePath());

            foreach (var file in files)
                if (Path.GetFileName(file).StartsWith(persistentIdentifier) == false)
                    File.Delete(file);

            files = Directory.GetFiles(Util.Util.GetInteriorSavePath());

            foreach (var file in files)
                if (Path.GetFileName(file).StartsWith(persistentIdentifier) == false)
                    File.Delete(file);
        }

        public static bool DoesWorldGridChunkExist(int chunkX, int chunkY)
        {
            var chunkPathPersistent = Path.Combine(Util.Util.GetWorldSavePath(), persistentIdentifier + (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                if (File.Exists(chunkPathPersistent) || File.Exists(chunkPath))
                {
                    return true;
                }

                return false;
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static void SaveInterior(Interior interior)
        {
            var chunkPathPersistent = Path.Combine(Util.Util.GetInteriorSavePath(), persistentIdentifier + interior.ID);
            var chunkPath = Path.Combine(Util.Util.GetInteriorSavePath(), interior.ID);

            fileLocks.Enter(chunkPath);

            try
            {
                var savePath = interior.IsPersistent ? chunkPathPersistent : chunkPath;

                using (var stream = File.OpenWrite(savePath))
                using (var writer = new BsonWriter(stream))
                {
                    InteriorSerializer.Serialize(interior).WriteTo(writer);
                }

                // We delete the other chunk
                var deletePath = interior.IsPersistent ? chunkPath : chunkPathPersistent;

                if (File.Exists(deletePath))
                    File.Delete(deletePath);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static void EraseInterior(Interior interior)
        {
            var chunkPathPersistent = Path.Combine(Util.Util.GetInteriorSavePath(), persistentIdentifier + interior.ID);
            var chunkPath = Path.Combine(Util.Util.GetInteriorSavePath(), interior.ID);

            fileLocks.Enter(chunkPath);

            try
            {
                var savePath = interior.IsPersistent ? chunkPathPersistent : chunkPath;

                File.Delete(savePath);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static Interior LoadInterior(string ID)
        {
            Debug.Assert(ID != Interior.Outside, "Cannot load outside interior!");

            var chunkPathPersistent = Path.Combine(Util.Util.GetInteriorSavePath(), persistentIdentifier + ID);
            var chunkPathPersistentExists = File.Exists(chunkPathPersistent);
            var chunkPath = Path.Combine(Util.Util.GetInteriorSavePath(), ID);
            
            if (!chunkPathPersistentExists && !File.Exists(chunkPath))
            {
                throw new Exception("Cannot find interior with ID " + ID);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                Interior interior;

                var loadPath = chunkPathPersistentExists ? chunkPathPersistent : chunkPath;

                using (var stream = File.OpenRead(loadPath))
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
            var chunkPathPersistent = Path.Combine(Util.Util.GetWalkableGridSavePath(), persistentIdentifier + (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                byte[] bytes;
                var savePath = chunk.IsPersistent ? chunkPathPersistent : chunkPath;

                chunk.copyDataTo(out bytes);
                File.WriteAllBytes(savePath, bytes);

                // We delete the other chunk
                var deletePath = chunk.IsPersistent ? chunkPath : chunkPathPersistent;

                if (File.Exists(deletePath))
                    File.Delete(deletePath);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static WalkableGridChunk LoadWalkableGridChunk(int chunkX, int chunkY)
        {
            var chunkPathPersistent = Path.Combine(Util.Util.GetWalkableGridSavePath(), persistentIdentifier + (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));
            var chunkPathPersistentExists = File.Exists(chunkPathPersistent);
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!chunkPathPersistentExists && !File.Exists(chunkPath))
            {
                SimulationGame.WorldGenerator.generateChunk(chunkX * WalkableGrid.WalkableGridBlockChunkSize.X, chunkY * WalkableGrid.WalkableGridBlockChunkSize.Y);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                var loadPath = chunkPathPersistentExists ? chunkPathPersistent : chunkPath;
                var content = File.ReadAllBytes(loadPath);

                return WalkableGridChunk.CreateChunkFrom(chunkX, chunkY, ref content);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }

        public static WorldGridChunk LoadWorldGridChunk(int chunkX, int chunkY)
        {
            var chunkPathPersistent = Path.Combine(Util.Util.GetWorldSavePath(), persistentIdentifier + (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));
            var chunkPathPersistentExists = File.Exists(chunkPathPersistent);
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            if (!chunkPathPersistentExists && !File.Exists(chunkPath))
            {
                SimulationGame.WorldGenerator.generateChunk(chunkX * WorldGrid.WorldChunkBlockSize.X, chunkY * WorldGrid.WorldChunkBlockSize.Y);
            }

            fileLocks.Enter(chunkPath);

            try
            {
                var loadPath = chunkPathPersistentExists ? chunkPathPersistent : chunkPath;
                WorldGridChunk worldGridChunk;

                using (var stream = File.OpenRead(loadPath))
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
            var chunkPathPersistent = Path.Combine(Util.Util.GetWorldSavePath(), persistentIdentifier + (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));
            var chunkPath = Path.Combine(Util.Util.GetWorldSavePath(), (chunkX < 0 ? "m" + Math.Abs(chunkX) : "" + chunkX) + "_" + (chunkY < 0 ? "m" + Math.Abs(chunkY) : "" + chunkY));

            fileLocks.Enter(chunkPath);

            try
            {
                var savePath = chunk.IsPersistent ? chunkPathPersistent : chunkPath;

                using (var stream = File.OpenWrite(savePath))
                using (var writer = new BsonWriter(stream))
                {
                    WorldGridChunkSerializer.Serialize(chunk).WriteTo(writer);
                }

                // We delete the other chunk
                var deletePath = chunk.IsPersistent ? chunkPath : chunkPathPersistent;

                if (File.Exists(deletePath))
                    File.Delete(deletePath);
            }
            finally
            {
                fileLocks.Exit(chunkPath);
            }
        }
    }
}
