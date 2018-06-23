using Microsoft.Xna.Framework;
using Simulation.Game.World.Generator;
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
        public static Point WalkableGridBlockChunkSize = new Point(96, 96);
        public static Point WalkableGridPixelChunkSize = new Point(WalkableGridBlockChunkSize.X * World.BlockSize.X, WalkableGridBlockChunkSize.Y * World.BlockSize.Y);

        public static int WalkableGridArrayChunkCount = WalkableGridBlockChunkSize.X * WalkableGridBlockChunkSize.Y / 32;

        private ConcurrentDictionary<string, WalkableGridChunk> walkableGrid = new ConcurrentDictionary<string, WalkableGridChunk>();
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
                walkableGrid[key] = WorldLoader.loadWalkableGridChunk(x, y);
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

        public bool IsPositionWalkable(int realX, int realY)
        {
            var blockPosition = GeometryUtils.getChunkPosition(realX, realY, World.BlockSize.X, World.BlockSize.Y);
            var chunkPosition = GeometryUtils.getChunkPosition(blockPosition.X, blockPosition.Y, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockPosition.X, blockPosition.Y, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.Y) == false)
            {
                waitTillChunkIsLoaded(chunkPosition.X, chunkPosition.Y);
            }

            return getBit(walkableGrid[chunkPosition.X + "," + chunkPosition.Y].chunkData[arrayPosition / 32], arrayPosition % 32);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.Y) == false)
            {
                waitTillChunkIsLoaded(chunkPosition.X, chunkPosition.Y);
            }

            return getBit(walkableGrid[chunkPosition.X + "," + chunkPosition.Y].chunkData[arrayPosition / 32], arrayPosition % 32);
        }

        public void changeBlockWalkable(int blockX, int blockY, bool notWalkable)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.X) == false)
                waitTillChunkIsLoaded(chunkPosition.X, chunkPosition.Y);

            setBit(ref walkableGrid[chunkPosition.X + "," + chunkPosition.Y].chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
        }

        public static void changeBlockInChunk(WalkableGridChunk chunk, int blockX, int blockY, bool notWalkable)
        {
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);

            setBit(ref chunk.chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
        }

        private static bool getBit(UInt32 x, int bitnum)
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
