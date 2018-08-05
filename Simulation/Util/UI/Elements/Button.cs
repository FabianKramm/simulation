using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simulation.Util.UI.Elements
{
    public class Button: UIElement
    {
        public static SpriteFont ButtonFont;

        private Vector2 stringBounds;
        private Point padding;
        
        private Primitive primitiveDrawer;
        private float depth;

        public bool ShowBorder = true;
        public Color TextColor = Color.White;
        public Color HoverColor = Color.Orange;
        public string Text
        {
            get; private set;
        }

        public Button(string text, Point position, Point? btnPadding = null, float depth = 1.0f)
        {
            this.Text = text;
            this.depth = depth;

            padding = btnPadding ?? new Point(10, 5);
            stringBounds = ButtonFont.MeasureString(text);

            Bounds = new Geometry.Rect(position.X, position.Y, (int)stringBounds.X + 2 * padding.X, (int)stringBounds.Y + 2 * padding.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(primitiveDrawer == null)
            {
                if(depth != 1.0f)
                {
                    primitiveDrawer = new Primitive(SimulationGame.Graphics.GraphicsDevice, spriteBatch);
                }
                else
                {
                    primitiveDrawer = SimulationGame.PrimitiveDrawer;
                }
            }

            var textPosition = new Vector2(Bounds.X + padding.X, Bounds.Y + padding.Y);

            if(ShowBorder)
                primitiveDrawer.Rectangle(Bounds.ToXnaRectangle(), IsHover ? HoverColor : TextColor);

            spriteBatch.DrawString(ButtonFont, Text, textPosition, IsHover ? HoverColor : TextColor);
        }
    }
}
