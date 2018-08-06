using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;

namespace Simulation.Game.Hud.WorldBuilder.ObjectListItems
{
    public class AmbientHitableObjectListItem: ObjectListItem
    {
        public AmbientHitableObjectType AmbientHitableObjectType
        {
            get; private set;
        }

        private string displayString;
        private Vector2 stringBounds;

        public AmbientHitableObjectListItem(AmbientHitableObjectType ambientHitableObjectType)
        {
            this.AmbientHitableObjectType = ambientHitableObjectType;
            this.displayString = ambientHitableObjectType.Name + " (" + ambientHitableObjectType.ID + ")";
            this.stringBounds = Button.ButtonFont.MeasureString(displayString);

            Bounds = new Rect(Point.Zero, ambientHitableObjectType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (AmbientHitableObjectType.SpritePath != null)
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(AmbientHitableObjectType.SpritePath),
                    Bounds.GetPositionVector(), new Rectangle(AmbientHitableObjectType.SpritePositions[0], AmbientHitableObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(Bounds.X + AmbientHitableObjectType.SpriteBounds.X + 20, Bounds.Y + AmbientHitableObjectType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }

        public override void DrawPreview(SpriteBatch spriteBatch, Vector2 position)
        {
            if (AmbientHitableObjectType.SpritePath != null)
            {
                var realPosition = new Vector2(position.X - AmbientHitableObjectType.SpriteOrigin.X, position.Y - AmbientHitableObjectType.SpriteOrigin.Y);

                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(AmbientHitableObjectType.SpritePath),
                    realPosition, new Rectangle(AmbientHitableObjectType.SpritePositions[0], AmbientHitableObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}
