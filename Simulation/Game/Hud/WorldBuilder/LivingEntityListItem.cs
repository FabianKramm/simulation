using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class LivingEntityListItem: UIElement
    {
        public LivingEntityType LivingEntityType
        {
            get; private set;
        }

        private string displayString;
        private Vector2 stringBounds;

        public LivingEntityListItem(LivingEntityType livingEntityType)
        {
            this.LivingEntityType = livingEntityType;
            this.displayString = livingEntityType.Name + " (" + livingEntityType.ID + ")";
            this.stringBounds = Button.ButtonFont.MeasureString(displayString);

            Bounds = new Rect(Point.Zero, livingEntityType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (LivingEntityType.SpritePath != null)
            {
                if(LivingEntityType.WithGrid)
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(LivingEntityType.SpritePath),
                        Bounds.GetPositionVector(), new Rectangle(new Point(LivingEntityType.DownAnimation[0].X * LivingEntityType.SpriteBounds.X, LivingEntityType.DownAnimation[0].Y * LivingEntityType.SpriteBounds.Y), LivingEntityType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(LivingEntityType.SpritePath),
                        Bounds.GetPositionVector(), new Rectangle(LivingEntityType.DownAnimation[0], LivingEntityType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
               
            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(Bounds.X + LivingEntityType.SpriteBounds.X + 20, Bounds.Y + LivingEntityType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }
    }
}
