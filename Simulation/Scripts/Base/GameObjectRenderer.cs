using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects;

namespace Simulation.Scripts.Base
{
    public interface GameObjectRenderer
    {
        void Init(GameObject gameObject);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
