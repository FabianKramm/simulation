using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.MetaData;
using Simulation.Util.UI;

namespace Simulation.Game.Hud.WorldBuilder.ObjectListItems
{
    public abstract class ObjectListItem: UIElement
    {
        public abstract MetaDataType GetObject();
    }
}
