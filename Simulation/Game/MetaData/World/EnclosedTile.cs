namespace Simulation.Game.MetaData.World
{
    // Tile Variations with transition tiles from the main tiles to the enclosed tile
    public class EnclosedTile
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
        public int BottomLeft;
        public int BottomRight;
        public int TopLeft;
        public int TopRight;
        public int Center;

        public bool HasType(BlockType blockType)
        {
            if (blockType.ID == Left ||
                blockType.ID == Right ||
                blockType.ID == Top ||
                blockType.ID == Bottom ||
                blockType.ID == BottomLeft ||
                blockType.ID == BottomRight ||
                blockType.ID == TopLeft ||
                blockType.ID == TopRight ||
                blockType.ID == Center)
            {
                return true;
            }

            return false;
        }
    }
}
