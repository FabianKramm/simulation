using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Util.Geometry;
using Simulation.Util.UI.Elements;
using System;

namespace Simulation.Game.Hud.WorldBuilder.ObjectListItems
{
    public class AmbientObjectListItem: ObjectListItem
    {
        public AmbientObjectType AmbientObjectType
        {
            get; private set;
        }

        private string displayString;
        private Vector2 stringBounds;

        public AmbientObjectListItem(AmbientObjectType ambientObjectType)
        {
            this.AmbientObjectType = ambientObjectType;
            this.displayString = ambientObjectType.Name + " (" + ambientObjectType.ID + ")";
            this.stringBounds = Button.ButtonFont.MeasureString(displayString);

            Bounds = new Rect(Point.Zero, ambientObjectType.SpriteBounds);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (AmbientObjectType.SpritePath != null)
                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(AmbientObjectType.SpritePath),
                    Bounds.GetPositionVector(), new Rectangle(AmbientObjectType.SpritePositions[0], AmbientObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.DrawString(Button.ButtonFont, displayString, new Vector2(Bounds.X + AmbientObjectType.SpriteBounds.X + 20, Bounds.Y + AmbientObjectType.SpriteBounds.Y / 2 - stringBounds.Y / 2), Color.White);
        }

        public override void DrawPreview(SpriteBatch spriteBatch, Vector2 position)
        {
            if (AmbientObjectType.SpritePath != null)
            {
                var realPosition = new Vector2(position.X - AmbientObjectType.SpriteOrigin.X, position.Y - AmbientObjectType.SpriteOrigin.Y);

                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(AmbientObjectType.SpritePath),
                    realPosition, new Rectangle(AmbientObjectType.SpritePositions[0], AmbientObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}
