using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Game.MetaData.World;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;

namespace Simulation.Game.Hud.WorldBuilder.ObjectListItems
{
    public class BlockListItem: ObjectListItem
    {
        public BlockType BlockType
        {
            get; private set;
        }

        private string displayString;
        private Vector2 stringBounds;

        public BlockListItem(BlockType blockType)
        {
            this.BlockType = blockType;
            this.displayString = blockType.Name + " (" + blockType.ID + ")";
            this.stringBounds = Button.ButtonFont.MeasureString(displayString);

            Bounds = new Rect(Point.Zero, blockType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(BlockType.SpritePath != null)
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(BlockType.SpritePath),
                    Bounds.GetPositionVector(), new Rectangle(BlockType.SpritePosition, BlockType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(Bounds.X + BlockType.SpriteBounds.X + 20, Bounds.Y + BlockType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }

        public override MetaDataType GetObject()
        {
            return BlockType;
        }
    }
}
