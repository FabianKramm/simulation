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
            return Path.Combine(getGameFolder(), InteriorSavePath);
        }

        public static string GetWorldSavePath()
        {
            return Path.Combine(getGameFolder(), WorldSavePath);
        }

        public static string GetWalkableGridSavePath()
        {
            return Path.Combine(getGameFolder(), WalkableGridSavePath);
        }

        public static void clearWorldFiles()
        {
            var gameFolder = getGameFolder();

            if(Directory.Exists(Path.Combine(gameFolder, WorldSavePath)))
            {
                Directory.Delete(Path.Combine(gameFolder, WorldSavePath), true);
            }
            
            if(Directory.Exists(Path.Combine(gameFolder, WalkableGridSavePath)))
            {
                Directory.Delete(Path.Combine(gameFolder, WalkableGridSavePath), true);
            }

            createGameFolders();
        }

        public static void createGameFolders()
        {
            var gameFolder = getGameFolder();

            Directory.CreateDirectory(gameFolder);
            Directory.CreateDirectory(Path.Combine(gameFolder, WorldSavePath));
            Directory.CreateDirectory(Path.Combine(gameFolder, WalkableGridSavePath));
        }

        public static string getGameFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimulationGame");
        }

        public static string getUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
