namespace Simulation.Game.MetaData.World
{
    public class ElevationTile
    {
        public int ElevatedLeft;
        public int ElevatedRight;
        public int ElevatedTop;
        public int ElevatedBottom;
        public int ElevatedCenter;

        public int ElevatedOuterCornerBottomLeft;
        public int ElevatedOuterCornerBottomRight;
        public int ElevatedOuterCornerTopLeft;
        public int ElevatedOuterCornerTopRight;

        public int ElevatedInnerCornerBottomLeft;
        public int ElevatedInnerCornerBottomRight;
        public int ElevatedInnerCornerTopLeft;
        public int ElevatedInnerCornerTopRight;

        public int WallLeft;
        public int WallMiddle;
        public int WallRight;

        public int WallBottomLeft;
        public int WallBottomMiddle;
        public int WallBottomRight;

        public bool HasType(BlockType blockType)
        {
            if (blockType.ID == ElevatedLeft ||
                blockType.ID == ElevatedRight ||
                blockType.ID == ElevatedTop ||
                blockType.ID == ElevatedBottom ||
                blockType.ID == ElevatedOuterCornerBottomLeft ||
                blockType.ID == ElevatedOuterCornerBottomRight ||
                blockType.ID == ElevatedOuterCornerTopLeft ||
                blockType.ID == ElevatedOuterCornerTopRight ||
                blockType.ID == ElevatedInnerCornerBottomLeft ||
                blockType.ID == ElevatedInnerCornerBottomRight ||
                blockType.ID == ElevatedInnerCornerTopLeft ||
                blockType.ID == ElevatedInnerCornerTopRight ||
                blockType.ID == ElevatedCenter ||
                blockType.ID == WallLeft ||
                blockType.ID == WallMiddle ||
                blockType.ID == WallRight ||
                blockType.ID == WallBottomLeft ||
                blockType.ID == WallBottomMiddle ||
                blockType.ID == WallBottomRight)
            {
                return true;
            }

            return false;
        }
    }
}
