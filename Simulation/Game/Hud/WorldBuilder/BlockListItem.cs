using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class BlockListItem: UIElement
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

            ClickBounds = new Rect(Point.Zero, blockType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(BlockType.SpritePath != null)
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(BlockType.SpritePath),
                    ClickBounds.GetPositionVector(), new Rectangle(BlockType.SpritePostion, BlockType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(ClickBounds.X + BlockType.SpriteBounds.X + 20, ClickBounds.Y + BlockType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }
    }
}
