using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Util
{
    public class CollisionUtils
    {
        public static BlockingType getBlockingTypeFromBlock(BlockType blockType)
        {
            switch(blockType)
            {
                case BlockType.GRASS_WATERHOLE:
                    return BlockingType.BLOCKING;
                default:
                    return BlockingType.NOT_BLOCKING;
            }
        }
    }
}
