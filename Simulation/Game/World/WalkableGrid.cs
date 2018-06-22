using Microsoft.Xna.Framework;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/*
 * The WalkableGrid is only used for quick pathfinding
 */
namespace Simulation.Game.World
{
    // TODO: Cleanup and save unused chunks
    public class WalkableGrid
    {
        public static Point WalkableGridChunkSize = new Point(96, 96);
        public static int WalkableGridChunkCount = WalkableGridChunkSize.X * WalkableGridChunkSize.Y / 32;

        private ConcurrentDictionary<string, UInt32[]> walkableGrid = new ConcurrentDictionary<string, UInt32[]>();
        private Dictionary<string, object> chunkLocks = new Dictionary<string, object>();

        private void waitTillChunkIsLoaded(int x, int y)
        {
            string key = x + "," + y;
            bool wait = false;
            object temp;

            lock (chunkLocks)
            {
                if (chunkLocks.ContainsKey(key) == true)
                {
                    wait = true;
                    temp = chunkLocks[key];
                }
                else
                {
                    temp = new object();
                    
                    chunkLocks[key] = temp;
                    Monitor.Enter(temp);
                }
            }

            if(wait)
            {
                Monitor.Enter(temp);
                Monitor.Exit(temp);
                return;
            }

            try
            {
                walkableGrid[key] = loadChunk(x, y);
            }
            finally
            {
                lock(chunkLocks)
                {
                    chunkLocks.Remove(key);
                    Monitor.Exit(temp);
                }
            }
        }

        public static void saveChunk(int x, int y, UInt32[] chunk)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (x < 0 ? "m" + Math.Abs(x) : "" + x) + "_" + (y < 0 ? "y" + Math.Abs(y) : "" + y));
            var bytes = new byte[chunk.Length * 4];

            Buffer.BlockCopy(chunk, 0, bytes, 0, chunk.Length * 4);

            File.WriteAllBytes(chunkPath, bytes);
        }

        private UInt32[] loadChunk(int x, int y)
        {
            var chunkPath = Path.Combine(Util.Util.GetWalkableGridSavePath(), (x < 0 ? "m" + Math.Abs(x) : "" + x) + "_" + (y < 0 ? "y" + Math.Abs(y) : "" + y));

            if(!File.Exists(chunkPath))
            {
                // Generate new World Chunk if it doesn't exist
                SimulationGame.worldGenerator.generateChunk(x * WalkableGridChunkSize.X, y * WalkableGridChunkSize.Y);
            }
            
            var content = File.ReadAllBytes(chunkPath);

            UInt32[] loadedChunk = new UInt32[WalkableGridChunkCount];

            Buffer.BlockCopy(content, 0, loadedChunk, 0, content.Length);

            return loadedChunk;
        }

        public bool IsPositionWalkable(int x, int y)
        {
            var chunkPosition = GeometryUtils.projectPosition(x, y, WalkableGridChunkSize.X, WalkableGridChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(x, y, WalkableGridChunkSize.X, WalkableGridChunkSize.Y);

            if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.Y) == false)
            {
                waitTillChunkIsLoaded(chunkPosition.X, chunkPosition.Y);
            }

            return getBit(walkableGrid[chunkPosition.X + "," + chunkPosition.Y][arrayPosition / 32], arrayPosition % 32);
        }

        public void changePositionWalkable(int x, int y, bool notWalkable)
        {
            var chunkPosition = GeometryUtils.projectPosition(x, y, WalkableGridChunkSize.X, WalkableGridChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(x, y, WalkableGridChunkSize.X, WalkableGridChunkSize.Y);

            if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.X) == false)
                waitTillChunkIsLoaded(chunkPosition.X, chunkPosition.Y);

            setBit(ref walkableGrid[chunkPosition.X + "," + chunkPosition.Y][arrayPosition / 32], arrayPosition % 32, notWalkable);
        }

        public static void changePositionInChunk(UInt32[] chunk, int x, int y, bool notWalkable)
        {
            var arrayPosition = GeometryUtils.getIndexFromPoint(x, y, WalkableGridChunkSize.X, WalkableGridChunkSize.Y);

            setBit(ref chunk[arrayPosition / 32], arrayPosition % 32, notWalkable);
        }

        public static bool getBit(UInt32 x, int bitnum)
        {
            return (x & (1 << bitnum)) != 0;
        }

        private static void setBit(ref UInt32 x, int bitnum, bool val)
        {
            if (val)
                x |= (UInt32)(1 << bitnum);
            else
                x &= ~(UInt32)(1 << bitnum);
        }
    }
}
