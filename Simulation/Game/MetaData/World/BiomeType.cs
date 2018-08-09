using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.MetaData.World
{
    public struct BiomeObjectProbability
    {
        public int ObjectId;
        public float Probability;
        public Point Bounds;
    }

    public struct BiomePointOfInterestProbability
    {
        public int PointOfInterestId;
        public float Probability;
    }

    public class BiomeType
    {
        public static Dictionary<int, BiomeType> lookup = new Dictionary<int, BiomeType>()
        {
            {0, new BiomeType
            {
                ID=0,
                MainTilesIds=new int[]
                {
                    1
                },
            }}
        };
        public static readonly BiomeType Plain = lookup[0];

        public int ID;

        public float ElevationProbability = 0;
        public float ElevationHeights = 0;
        public int ElevationInterpolation = 16;
        public bool NegativeElevationAllowed = false;

        public EnclosedTile StreetTileIds = null;
        public EnclosedTile RiverTileIds = null;

        public int[] MainTilesIds;
        public EnclosedTile[] VariationTileIds = null;

        public ElevationTile[] DoubleNegativeElevationTileIds = null;
        public ElevationTile[] NegativeElevationTileIds = null;
        public ElevationTile[] ElevationTileIds = null;
        public ElevationTile[] DoubleElevationTileIds = null;

        public BiomeObjectProbability[] DecorationObjectIds = null;
        public BiomeObjectProbability[] DecorationHitableObjectIds = null;
        public BiomeObjectProbability[] LivingEntitiyIds = null;

        public BiomePointOfInterestProbability[] PointOfInterestsId = null;
    }
}
