using Newtonsoft.Json;
using System;

namespace Simulation.Game.World
{
    public class WalkableGridChunk
    {
        public UInt32[] chunkData;
        private WalkableGridChunk() {}

        public static WalkableGridChunk createEmpty()
        {
            WalkableGridChunk walkableGridChunk = new WalkableGridChunk();

            walkableGridChunk.chunkData = new UInt32[WalkableGrid.WalkableGridArrayChunkCount];

            return walkableGridChunk;
        }

        public static WalkableGridChunk createChunkFrom(ref byte[] fromData)
        {
            WalkableGridChunk walkableGridChunk = new WalkableGridChunk();

            walkableGridChunk.chunkData = new UInt32[WalkableGrid.WalkableGridArrayChunkCount];

            Buffer.BlockCopy(fromData, 0, walkableGridChunk.chunkData, 0, fromData.Length);

            return walkableGridChunk;
        }

        public void copyDataTo(out byte[] retData)
        {
            retData = new byte[chunkData.Length * 4];

            Buffer.BlockCopy(chunkData, 0, retData, 0, chunkData.Length * 4);
        }
    }
}
