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
        private BlockType blockType;
        private string displayString;
        private Vector2 stringBounds;

        public BlockListItem(BlockType blockType)
        {
            this.blockType = blockType;
            this.displayString = blockType.Name + " (" + blockType.ID + ")";
            this.stringBounds = Button.ButtonFont.MeasureString(displayString);

            ClickBounds = new Rect(Point.Zero, blockType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(blockType.SpritePath != null)
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(blockType.SpritePath),
                    ClickBounds.GetPositionVector(), new Rectangle(blockType.SpritePostion, blockType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(ClickBounds.X + blockType.SpriteBounds.X + 20, ClickBounds.Y + blockType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }
    }
}
