using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public class World
    {
        private Block[,] grid;
        public static Point dimensions = new Point(100, 100);

        public static Point BlockSize = new Point(32, 32);
        public static Point WorldChunkSize = new Point(32, 32);

        public static int RenderOuterBlockRange = 3;

        private Dictionary<(int, int), WorldGridChunk> worldGrid = new Dictionary<(int, int), WorldGridChunk>();
        private Dictionary<(int, int), TaskCompletionSource<bool>> chunksLoading = new Dictionary<(int, int), TaskCompletionSource<bool>>();

        private Dictionary<string, DrawableObject> drawableObjects;
        private WalkableGrid walkableGrid = new WalkableGrid();

        private void loadChunk(int x, int y)
        {
            var chunkPath = ""; // worldSavePath + (x < 0 ? "m" + Math.Abs(x) : "" + x) + (y < 0 ? "y" + Math.Abs(y) : "" + y);

            if (!File.Exists(chunkPath))
            {
                // Generate new World Chunk if it doesn't exist
                SimulationGame.worldGenerator.generateChunk(x * WorldChunkSize.X, y * WorldChunkSize.Y);
            }
        }

        public static void saveChunk(int x, int y, WorldGridChunk chunk)
        {
            var chunkPath = ""; // worldSavePath + (x < 0 ? "m" + Math.Abs(x) : "" + x) + (y < 0 ? "y" + Math.Abs(y) : "" + y);
        }

        public List<Block> getTouchedWorldBlocks(ref Rectangle rect)
        {
            List<Block> retList = new List<Block>();
            int worldWidth = dimensions.X * BlockSize.X;
            int worldHeight = dimensions.Y * BlockSize.Y;

            int left = Math.Max(0, rect.Left - rect.Left % BlockSize.X);
            int right = Math.Min(worldWidth, rect.Right + (BlockSize.X - rect.Right % BlockSize.X));

            int startTop = Math.Max(0, rect.Top - rect.Top % BlockSize.Y);
            int bottom = Math.Min(worldHeight, rect.Bottom + (BlockSize.Y - rect.Bottom % BlockSize.Y));

            for (; left < right; left += BlockSize.X)
                for (int top = startTop; top < bottom; top += BlockSize.Y)
                    retList.Add(grid[left / BlockSize.X, top / BlockSize.Y]);

            return retList;
        }

        public bool canMove(Rectangle rect)
        {
            int worldWidth = dimensions.X * BlockSize.X;
            int worldHeight = dimensions.Y * BlockSize.Y;

            int left = Math.Max(0, rect.Left - rect.Left % BlockSize.X);
            int right = Math.Min(worldWidth, rect.Right + (BlockSize.X - rect.Right % BlockSize.X));

            int startTop = Math.Max(0, rect.Top - rect.Top % BlockSize.Y);
            int bottom = Math.Min(worldHeight, rect.Bottom + (BlockSize.Y - rect.Bottom % BlockSize.Y));

            for (; left < right; left += BlockSize.X)
                for (int top = startTop; top < bottom; top += BlockSize.Y)
                {
                    Block block = grid[left / BlockSize.X, top / BlockSize.Y];

                    if (block.blockingType == BlockingType.BLOCKING)
                        return false;

                    if (block.hitableObjects != null) 
                        foreach (HitableObject hitableObject in block.hitableObjects)
                            if (hitableObject.blockingType == BlockingType.BLOCKING && hitableObject.blockingBounds.Intersects(rect))
                                return false;
                }

            return true;
        }

        public void addHitableObject(HitableObject hitableObject)
        {
            if(hitableObject.useSameBounds)
            {
                List<Block> blocks = getTouchedWorldBlocks(ref hitableObject.hitBoxBounds);

                foreach (Block block in blocks)
                    block.addHitableObject(hitableObject);
            }
            else
            {
                Rectangle union = Rectangle.Union(hitableObject.hitBoxBounds, hitableObject.blockingBounds);
                List<Block> blocks = getTouchedWorldBlocks(ref union);

                foreach (Block block in blocks)
                    block.addHitableObject(hitableObject);
            }
        }

        public void removeHitableObject(HitableObject hitableObject)
        {
            if (hitableObject.useSameBounds)
            {
                List<Block> blocks = getTouchedWorldBlocks(ref hitableObject.hitBoxBounds);

                foreach (Block block in blocks)
                    block.removeHitableObject(hitableObject);
            }
            else
            {
                Rectangle union = Rectangle.Union(hitableObject.hitBoxBounds, hitableObject.blockingBounds);
                List<Block> blocks = getTouchedWorldBlocks(ref union);

                foreach (Block block in blocks)
                    block.removeHitableObject(hitableObject);
            }
        }

        public Block GetBlock(int x, int y)
        {
            return grid[x, y];
        }
    }
}
