using System.Collections.Generic;

namespace Simulation.Game.Generator
{
    public class BlockInformation
    {
        public int BlockId;

        public bool IsNotElevated;
        public bool IsDoubleElevated;
        public bool IsElevated;
        public bool IsNegativeElevated;
        public bool IsDoubleNegativeElevated;

        public bool IsStreet;
        public bool IsRiver;
        public bool IsReserved;
        public bool IsBlocked;

        public List<int> ContainedAmbientObjects;
        public List<int> ContainedAmbientHitableObjects;
        public List<int> ContainedLivingEntities;
    }
}
