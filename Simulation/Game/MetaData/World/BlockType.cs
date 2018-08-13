using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.MetaData.World
{
    public class BlockType: MetaDataType
    {
        public static readonly int Invalid = -1;
        public static readonly int None = 0;
        public static Dictionary<int, BlockType> lookup = new Dictionary<int, BlockType>()
        {
            {0, new BlockType()
            {
                ID=0,
                Name="None",
                BlockingType=BlockingType.BLOCKING,
                HitBoxType=HitBoxType.HITABLE_BLOCK
            }},
            {1, new BlockType()
            {
                ID=1,
                Name="Grass",
                SpritePath=@"Tiles\Exterior01",
                SpritePosition=new Point(0, 0)
            }},
        };

        public int ID;
        public string Name = null;

        public BlockingType BlockingType = BlockingType.NOT_BLOCKING;
        public HitBoxType HitBoxType = HitBoxType.NO_HITBOX;

        // Render
        public string SpritePath = null;
        public Point SpritePosition = Point.Zero;
        public Point SpriteBounds = new Point(WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
    }
}
