/*
Requirements:
    - Travel through large area
    - Enable background actions for some npcs
 
 */
using Simulation.Game.MetaData;
using Simulation.Game.World;

namespace Simulation.Game.Objects
{
    public class AmbientObject: GameObject
    {
        public int AmbientObjectType;

        // Json
        protected AmbientObject(): base() { }

        public AmbientObject(WorldPosition worldPosition): base(worldPosition) { }
    }
}
