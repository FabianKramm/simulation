﻿using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using Simulation.Game.World;

namespace Simulation.Game.Objects
{
    public class AmbientObject: GameObject
    {
        [Serialize]
        public int AmbientObjectType;

        // Json
        protected AmbientObject(): base() { }

        public AmbientObject(WorldPosition worldPosition): base(worldPosition) { }

        public AmbientObjectType GetObjectType()
        {
            return MetaData.AmbientObjectType.lookup[AmbientObjectType];
        }
    }
}
