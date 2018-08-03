using Microsoft.Xna.Framework;

namespace Simulation.Game.Objects.Interfaces
{
    public interface GameObjectController
    {
        GameObjectController Create(GameObject gameObject);
        
        void Update(GameTime gameTime);
    }
}
