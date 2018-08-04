using Microsoft.Xna.Framework;

namespace Simulation.Game.Objects.Interfaces
{
    public interface GameObjectController
    {
        void Init(GameObject gameObject);
        void Update(GameTime gameTime);
    }
}
