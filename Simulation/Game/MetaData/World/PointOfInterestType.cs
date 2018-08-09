using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.MetaData.World
{
    public class PointOfInterestType
    {
        public static readonly PointOfInterestType None = null;
        public static Dictionary<int, PointOfInterestType> lookup = new Dictionary<int, PointOfInterestType>();

        public int ID;
        public Point Bounds;

        public bool ShouldIncludeDecorationObjects = false;
        public bool ShouldIncludeDecorationHitableObjects = false;
        public bool ShouldIncludeLivingEntities = false;

        public bool ElevationAllowed = false;
        public bool RiversAllowed = false;
        public bool StreetsAllowed = false;

        public string PointOfInterestGenerator = null;
    }
}
