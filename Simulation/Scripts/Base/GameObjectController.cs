using Microsoft.Xna.Framework;
using Simulation.Game.Objects;

namespace Scripts.Base
{
    public interface GameObjectController
    {
        void Init(GameObject gameObject);
        void Update(GameTime gameTime);
    }
}
