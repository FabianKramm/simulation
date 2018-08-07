using Microsoft.Xna.Framework;
using Simulation.Game.Objects;

namespace Simulation.Scripts.Base
{
    public interface GameObjectController
    {
        void Init(GameObject gameObject);
        void Update(GameTime gameTime);
    }
}
