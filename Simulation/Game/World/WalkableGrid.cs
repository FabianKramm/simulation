using Microsoft.Xna.Framework;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        private Dictionary<string, WalkableGridChunk> walkableGrid = new Dictionary<string, WalkableGridChunk>();
        private NamedLock chunkLocks = new NamedLock();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void loadGridChunk(int chunkX, int chunkY)
        {
            walkableGrid[chunkX + "," + chunkY] = WorldLoader.loadWalkableGridChunk(chunkX, chunkY);
        }

        public void loadGridChunkAsync(int chunkX, int chunkY)
        {
            if(IsChunkLoaded(chunkX, chunkY) == false)
            {
                Task.Run(() =>
                {
                    var key = chunkX + "," + chunkY;

                    chunkLocks.Enter(key);

                    try
                    {
                        if (walkableGrid.ContainsKey(key) == false)
                            walkableGrid[key] = WorldLoader.loadWalkableGridChunk(chunkX, chunkY);
                    }
                    finally
                    {
                        chunkLocks.Exit(key);
                    }
                });
            }
        }

        private bool IsChunkLoaded(int chunkX, int chunkY)
        {
            var key = chunkX + "," + chunkY;

            chunkLocks.Enter(key);

            try
            {
                return walkableGrid.ContainsKey(key);
            }
            finally
            {
                chunkLocks.Exit(key);
            }
        }

        public bool IsPositionWalkable(int realX, int realY)
        {
            var blockPosition = GeometryUtils.getChunkPosition(realX, realY, World.BlockSize.X, World.BlockSize.Y);

            return IsBlockWalkable(blockPosition.X, blockPosition.Y);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var key = chunkPosition.X + "," + chunkPosition.Y;

            chunkLocks.Enter(key);

            try
            {
                if (walkableGrid.ContainsKey(key) == false)
                    walkableGrid[key] = WorldLoader.loadWalkableGridChunk(chunkPosition.X, chunkPosition.Y);

                return getBit(walkableGrid[key].chunkData[arrayPosition / 32], arrayPosition % 32);
            }
            finally
            {
                chunkLocks.Exit(key);
            }
        }

        public void setBlockWalkable(int blockX, int blockY, bool notWalkable)
        {
            var chunkPosition = GeometryUtils.getChunkPosition(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var arrayPosition = GeometryUtils.getIndexFromPoint(blockX, blockY, WalkableGridBlockChunkSize.X, WalkableGridBlockChunkSize.Y);
            var key = chunkPosition.X + "," + chunkPosition.Y;

            chunkLocks.Enter(key);

            try
            {
                if (walkableGrid.ContainsKey(chunkPosition.X + "," + chunkPosition.X) == false)
                    walkableGrid[key] = WorldLoader.loadWalkableGridChunk(chunkPosition.X, chunkPosition.Y);

                setBit(ref walkableGrid[chunkPosition.X + "," + chunkPosition.Y].chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
            }
            finally
            {
                chunkLocks.Exit(key);
            }
        }

        // Don't care about concurrency
        public static void setBlockWalkableInChunk(WalkableGridChunk chunk, int blockX, int blockY, bool notWalkable)
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
