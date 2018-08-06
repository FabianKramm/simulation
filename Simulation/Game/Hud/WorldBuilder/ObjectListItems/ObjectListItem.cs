using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Util.UI;

namespace Simulation.Game.Hud.WorldBuilder.ObjectListItems
{
    public abstract class ObjectListItem: UIElement
    {
        public abstract void DrawPreview(SpriteBatch spriteBatch, Vector2 position);
    }
}
