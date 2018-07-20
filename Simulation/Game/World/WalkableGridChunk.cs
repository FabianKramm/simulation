using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.World
{
    public class WalkableGridChunk
    {
        public bool IsPersistent
        {
            get; private set;
        }

        private UInt32[] chunkData;
        public Rect realChunkBounds
        {
            get; private set;
        }

        private WalkableGridChunk(int chunkX, int chunkY)
        {
            realChunkBounds = new Rect(chunkX * WalkableGrid.WalkableGridPixelChunkSize.X, chunkY * WalkableGrid.WalkableGridPixelChunkSize.Y, WalkableGrid.WalkableGridPixelChunkSize.X, WalkableGrid.WalkableGridPixelChunkSize.Y);
        }

        public void SetPersistent(bool persistent)
        {
            IsPersistent = persistent;
        }

        public static WalkableGridChunk createEmpty(int chunkX, int chunkY)
        {
            WalkableGridChunk walkableGridChunk = new WalkableGridChunk(chunkX, chunkY);

            walkableGridChunk.chunkData = new UInt32[WalkableGrid.WalkableGridArrayChunkCount];

            return walkableGridChunk;
        }

        public static WalkableGridChunk createChunkFrom(int chunkX, int chunkY, ref byte[] fromData)
        {
            WalkableGridChunk walkableGridChunk = new WalkableGridChunk(chunkX, chunkY);

            walkableGridChunk.chunkData = new UInt32[WalkableGrid.WalkableGridArrayChunkCount];

            Buffer.BlockCopy(fromData, 0, walkableGridChunk.chunkData, 0, fromData.Length);

            return walkableGridChunk;
        }

        public void copyDataTo(out byte[] retData)
        {
            retData = new byte[chunkData.Length * 4];

            Buffer.BlockCopy(chunkData, 0, retData, 0, chunkData.Length * 4);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            var arrayPosition = GeometryUtils.GetIndexFromPoint(blockX, blockY, WalkableGrid.WalkableGridBlockChunkSize.X, WalkableGrid.WalkableGridBlockChunkSize.Y);

            return !getBit(chunkData[arrayPosition / 32], arrayPosition % 32);
        }

        public void SetWalkable(int blockX, int blockY, bool notWalkable)
        {
            var arrayPosition = GeometryUtils.GetIndexFromPoint(blockX, blockY, WalkableGrid.WalkableGridBlockChunkSize.X, WalkableGrid.WalkableGridBlockChunkSize.Y);


            setBit(ref chunkData[arrayPosition / 32], arrayPosition % 32, notWalkable);
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
