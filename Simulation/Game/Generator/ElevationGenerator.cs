using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.Generator
{
    public static class ElevationGenerator
    {
        public static void GenerateHeightMap(WorldSegmentPlanner w)
        {
            var elevationCap = GeometryUtils.Clamp(w.Biome.ElevationProbability, 0, 1) * 0.6f;
            var deelevationCap = 0.2f;
            var completeDeelevationCap = 0.2f;

            if (elevationCap == 0.0f)
                return;

            for (int y = WorldSegmentPlanner.WorldSegmentBlockSize.Y - 1; y >= 0; y--)
                for (int x = WorldSegmentPlanner.WorldSegmentBlockSize.X - 1; x >= 0; x--)
                {
                    if (w.Blocks[x, y] == null)
                        continue;

                    if (w.Blocks[x, y].ElevationHeight != 0)
                        continue;

                    
                }

            /*var elevationCap = Math.Round(1.0f - GeometryUtils.Clamp(w.Biome.ElevationProbability, 0, 1) * 0.85f, 2);

            if (elevationCap == 1.0f)
                return;

            for (int x = 0; x < WorldSegmentPlanner.WorldSegmentBlockSize.X; x++)
                for (int y = 0; y < WorldSegmentPlanner.WorldSegmentBlockSize.Y; y++)
                {
                    if (w.Blocks[x, y] == null)
                        continue;

                    float avg = 0.0f;

                    for (int i = 0; i < w.Biome.ElevationInterpolation; i++)
                        for (int j = 0; j < w.Biome.ElevationInterpolation; j++)
                            avg += Math.Abs(w.NoiseGenerator.GetPerlin(x * w.Biome.ElevationInterpolation + i, y * w.Biome.ElevationInterpolation + j));

                    var noise = Math.Round(avg / (w.Biome.ElevationInterpolation * w.Biome.ElevationInterpolation), 1);

                    if (noise >= elevationCap)
                        w.Blocks[x, y].ElevationHeight = (int)Math.Floor(noise * 10);
                }

            flattenHeightMap(w);
            // reformatHeightMap(w);
            // finalizeHeightMap(w);*/
        }

        private static bool finalizeHeightMap(WorldSegmentPlanner w)
        {
            bool changedBlock = false;

            for (int y = WorldSegmentPlanner.WorldSegmentBlockSize.Y - 1; y >= 0; y--)
                for (int x = WorldSegmentPlanner.WorldSegmentBlockSize.X - 1; x >= 0; x--)
                {
                    if (w.Blocks[x, y] == null)
                        continue;

                    var currentHeight = w.Blocks[x, y].ElevationHeight;

                    if (currentHeight == 0)
                        continue;

                    var neighbors = w.GetNeighbors(x, y);

                    // 0 0 0
                    // 1 1 1
                    // 0 1 0
                    if (neighbors[1].ElevationHeight == currentHeight &&
                        neighbors[4].ElevationHeight == currentHeight &&
                        neighbors[6].ElevationHeight == currentHeight && 
                        neighbors[3].ElevationHeight < currentHeight && 
                        neighbors[2].ElevationHeight < currentHeight)
                    {
                        neighbors[2].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    else if(neighbors[1].ElevationHeight == currentHeight &&
                            neighbors[4].ElevationHeight == currentHeight &&
                            neighbors[6].ElevationHeight == currentHeight && 
                            neighbors[3].ElevationHeight < currentHeight && 
                            neighbors[7].ElevationHeight < currentHeight)
                    {
                        neighbors[7].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    // 0 1 0
                    // 1 1 0
                    // 0 1 0
                    else if (neighbors[1].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight && 
                             neighbors[6].ElevationHeight < currentHeight && 
                             neighbors[0].ElevationHeight < currentHeight)
                    {
                        neighbors[0].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    else if (neighbors[1].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight && 
                             neighbors[6].ElevationHeight < currentHeight && 
                             neighbors[2].ElevationHeight < currentHeight)
                    {
                        neighbors[2].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    // 0 1 0
                    // 0 1 1
                    // 0 1 0
                    else if (neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight &&
                             neighbors[6].ElevationHeight == currentHeight && 
                             neighbors[1].ElevationHeight < currentHeight && 
                             neighbors[5].ElevationHeight < currentHeight)
                    {
                        neighbors[5].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    else if (neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight &&
                             neighbors[6].ElevationHeight == currentHeight && 
                             neighbors[1].ElevationHeight < currentHeight && 
                             neighbors[7].ElevationHeight < currentHeight)
                    {
                        neighbors[7].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    // 0 1 0
                    // 1 1 1
                    // 0 0 0
                    else if (neighbors[1].ElevationHeight == currentHeight && 
                             neighbors[6].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight && 
                             neighbors[4].ElevationHeight < currentHeight && 
                             neighbors[0].ElevationHeight < currentHeight)
                    {
                        neighbors[0].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    else if (neighbors[1].ElevationHeight == currentHeight &&
                             neighbors[6].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight && 
                             neighbors[4].ElevationHeight < currentHeight && 
                             neighbors[5].ElevationHeight < currentHeight)
                    {
                        neighbors[5].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    // 0 1 1
                    // 1 1 1
                    // 1 1 0
                    else if (neighbors[1].ElevationHeight == currentHeight &&
                             neighbors[2].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight &&
                             neighbors[5].ElevationHeight == currentHeight &&
                             neighbors[6].ElevationHeight == currentHeight &&
                             neighbors[0].ElevationHeight < currentHeight &&
                             neighbors[7].ElevationHeight < currentHeight)
                    {
                        neighbors[0].ElevationHeight = currentHeight;
                        neighbors[7].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                    // 1 1 0
                    // 1 1 1
                    // 0 1 1
                    else if (neighbors[0].ElevationHeight == currentHeight &&
                             neighbors[1].ElevationHeight == currentHeight &&
                             neighbors[3].ElevationHeight == currentHeight &&
                             neighbors[4].ElevationHeight == currentHeight &&
                             neighbors[6].ElevationHeight == currentHeight &&
                             neighbors[7].ElevationHeight == currentHeight &&
                             neighbors[2].ElevationHeight < currentHeight &&
                             neighbors[5].ElevationHeight < currentHeight)
                    {
                        neighbors[2].ElevationHeight = currentHeight;
                        neighbors[5].ElevationHeight = currentHeight;
                        changedBlock = true;
                    }
                }

            if (changedBlock == true)
            {
                finalizeHeightMap(w);

                return true;
            }

            return false;
        }

        private static bool flattenHeightMap(WorldSegmentPlanner w)
        {
            bool changedHeight = false;

            for (int y = WorldSegmentPlanner.WorldSegmentBlockSize.Y - 1; y >= 0; y--)
                for (int x = WorldSegmentPlanner.WorldSegmentBlockSize.X - 1; x >= 0; x--)
                {
                    if (w.Blocks[x, y] == null)
                        continue;

                    var currentHeight = w.Blocks[x, y].ElevationHeight;

                    if (currentHeight == 0)
                        continue;

                    var neighbors = w.GetNeighbors(x, y);

                    
                }

            if (changedHeight == true)
            {
                flattenHeightMap(w);

                return true;
            }

            return false;
        }

        private static bool isWallStart(int num)
        {
            return num % 3 == 1;
        }

        private static bool isWall(int num)
        {
            return num % 3 == 2;
        }

        private static bool isPlatform(int num)
        {
            return num % 3 == 0;
        }

        private static int getHeight(WorldSegmentPlanner w, int blockX, int blockY)
        {
            if(blockX < 0 || blockX >= WorldSegmentPlanner.WorldSegmentBlockSize.X || blockY < 0 || blockY >= WorldSegmentPlanner.WorldSegmentBlockSize.Y || w.Blocks[blockX, blockY] == null)
            {
                return 0;
            }

            return w.Blocks[blockX, blockY].ElevationHeight;
        }

        private static void setHeight(WorldSegmentPlanner w, int blockX, int blockY, int value)
        {
            if (blockX < 0 || blockX >= WorldSegmentPlanner.WorldSegmentBlockSize.X || blockY < 0 || blockY >= WorldSegmentPlanner.WorldSegmentBlockSize.Y || w.Blocks[blockX, blockY] == null)
            {
                return;
            }

            w.Blocks[blockX, blockY].ElevationHeight = value;
        }

        private static bool reformatHeightMap(WorldSegmentPlanner w)
        {
            bool somethingChanged = false;

            for (int y = 0; y < WorldSegmentPlanner.WorldSegmentBlockSize.Y; y++)
                for (int x = 0; x < WorldSegmentPlanner.WorldSegmentBlockSize.X; x++)
                {
                    if (w.Blocks[x, y] == null)
                        continue;

                    var currentHeight = w.Blocks[x, y].ElevationHeight;

                    if (currentHeight == 0)
                        continue;

                    var neighbors = w.GetNeighbors(x, y);


                }

            if (somethingChanged)
            {
                reformatHeightMap(w);

                return true;
            }

            return false;
        }

        public static void PrintHeightMap(WorldSegmentPlanner w)
        {
            for (int x = 0; x < WorldSegmentPlanner.WorldSegmentBlockSize.X; x++)
            {
                for (int y = 0; y < WorldSegmentPlanner.WorldSegmentBlockSize.Y; y++)
                {
                    if(w.Blocks[y, x] == null)
                    {
                        Console.Write("# ");
                    }
                    else
                    {
                        Console.Write(w.Blocks[y, x].ElevationHeight + " ");
                    }
                }

                Console.Write("\n");
            }
        }
    }
}
