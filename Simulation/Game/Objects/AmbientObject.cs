using Simulation.Game.Enums;
using Simulation.Game.World;

/*
Requirements:
    - Travel through large area
    - Enable background actions for some npcs
 
 */
namespace Simulation.Game.Objects
{
    public class AmbientObject: GameObject
    {
        public AmbientObjectType AmbientObjectType
        {
            get; private set;
        }

        // Create from JSON
        protected AmbientObject() {}

        public AmbientObject(AmbientObjectType ambientObjectType, WorldPosition position, bool hasDepth = false) :base(position)
        {
            AmbientObjectType = ambientObjectType;
        }
    }
}
