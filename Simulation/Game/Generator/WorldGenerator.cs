using Microsoft.Xna.Framework;
using System;
using Simulation.Game.MetaData.World;

namespace Simulation.Game.Generator
{
    /*
     * Planner: 
     *   - Biome
     *   - Streets
     *   - Rivers
     *   - Decoration
     *   - Decide Places for POI
     *   - Animals / Base Population
     *   
     * Biome (MetaData):
     *   - Biome
     *   - NormalBlocks
     *   - ElevationBlocks
     *   - StreetBlocks
     *   - RiverBlocks
     *   - DecorationObjects
     *   - LivingEntities 
     * 
     * Points of Interest (MetaData):
     *   - Biome Occurances
     *   - Size
     *   - Probability
     *   - Should Place Decoration Objects & Entities within Area
     *   - Rivers & Streets allowed
     *   
     *  Order of Execution:
     *   - Elevation
     *   - Streets + Rivers
     *   - Draw Blocks & Noise
     *   - Reserve Space for persistent Blocks & POI
     *   - Add Decoration Objects
     *   - Add Living Entities
     *   - Place POI
     */
    public class WorldGenerator
    {
        private Random random;
        private object generatorLock = new object();

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public void generateChunk(int blockX, int blockY)
        {
            lock(generatorLock)
            {
                generateWorldSegment(blockX, blockY);
            }
        }

        private void generateWorldSegment(int blockX, int blockY)
        {
            var worldSegment = new WorldSegmentPlanner(random.Next(0, 50000), new Point(blockX, blockY), BiomeType.Plain);

            if(worldSegment.Init() == false)
            {
                return; // Segment does already exist
            }

            worldSegment.Generate();
            worldSegment.Build();
        }
    }
}
