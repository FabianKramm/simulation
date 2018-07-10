using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.World
{
    public class WalkableGridChunk
    {
        public UInt32[] chunkData;
        public Rect realChunkBounds
        {
            get; private set;
        }

        private WalkableGridChunk(int chunkX, int chunkY)
        {
            realChunkBounds = new Rect(chunkX * WalkableGrid.WalkableGridPixelChunkSize.X, chunkY * WalkableGrid.WalkableGridPixelChunkSize.Y, WalkableGrid.WalkableGridPixelChunkSize.X, WalkableGrid.WalkableGridPixelChunkSize.Y);
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
    }
}
