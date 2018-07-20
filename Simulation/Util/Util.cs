using System;
using System.IO;

namespace Simulation.Util
{
    public class Util
    {
        private static string InteriorSavePath = @"World\Interior\";
        private static string WorldSavePath = @"World\Data\";
        private static string WalkableGridSavePath = @"World\WalkableGrid\";

        public static string GetInteriorSavePath()
        {
            return Path.Combine(GetGameFolder(), InteriorSavePath);
        }

        public static string GetWorldSavePath()
        {
            return Path.Combine(GetGameFolder(), WorldSavePath);
        }

        public static string GetWalkableGridSavePath()
        {
            return Path.Combine(GetGameFolder(), WalkableGridSavePath);
        }

        public static void clearWorldFiles()
        {
            var gameFolder = GetGameFolder();

            if(Directory.Exists(Path.Combine(gameFolder, WorldSavePath)))
            {
                Directory.Delete(Path.Combine(gameFolder, WorldSavePath), true);
            }
            
            if(Directory.Exists(Path.Combine(gameFolder, WalkableGridSavePath)))
            {
                Directory.Delete(Path.Combine(gameFolder, WalkableGridSavePath), true);
            }

            if (Directory.Exists(Path.Combine(gameFolder, InteriorSavePath)))
            {
                Directory.Delete(Path.Combine(gameFolder, InteriorSavePath), true);
            }

            CreateGameFolders();
        }

        public static void CreateGameFolders()
        {
            var gameFolder = GetGameFolder();

            Directory.CreateDirectory(gameFolder);
            Directory.CreateDirectory(Path.Combine(gameFolder, WorldSavePath));
            Directory.CreateDirectory(Path.Combine(gameFolder, WalkableGridSavePath));
            Directory.CreateDirectory(Path.Combine(gameFolder, InteriorSavePath));
        }

        public static string GetGameFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimulationGame");
        }

        public static string GetUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
