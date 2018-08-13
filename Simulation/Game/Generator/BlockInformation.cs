using System.Collections.Generic;

namespace Simulation.Game.Generator
{
    public class BlockInformation
    {
        public static BlockInformation None = new BlockInformation()
        {
            BlockId = -1,
            ElevationHeight = 0,
            IsReserved=true,
            IsBlocked = true,
        };

        public int BlockId;

        public int ElevationHeight = 0;

        public bool IsStreet;
        public bool IsRiver;
        public bool IsReserved;
        public bool IsBlocked;

        public List<int> ContainedAmbientObjects;
        public List<int> ContainedAmbientHitableObjects;
        public List<int> ContainedLivingEntities;
    }
}
