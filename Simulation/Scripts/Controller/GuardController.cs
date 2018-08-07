using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Scripts.Base;

namespace Simulation.Scripts.Controller
{
    public class GuardController : GameObjectController
    {
        private GuardAI guardAI;

        public void Init(GameObject gameObject)
        {
            guardAI = new GuardAI((MovingEntity)gameObject,
                gameObject.GetOrAddCustomProperty<WorldPosition>("GuardBlockPosition", gameObject.Position.ToBlockPosition()),
                gameObject.GetOrAddCustomProperty<Vector2>("LookDirection", new Vector2(0, 1)));
        }

        public void Update(GameTime gameTime)
        {
            guardAI.Update(gameTime);
        }
    }
}
