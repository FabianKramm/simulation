using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.Game.Objects.Interfaces
{
    public interface GameObjectRenderer
    {
        void Init(GameObject gameObject);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
