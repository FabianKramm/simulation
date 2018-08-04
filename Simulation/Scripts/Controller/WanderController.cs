using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.AI;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Objects.Interfaces;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using System;

class WanderController: GameObjectController
{
    private WanderAI wanderAI;

    public void Init(GameObject gameObject)
    {
        wanderAI = new WanderAI((MovingEntity)gameObject, 
            gameObject.GetOrAddCustomProperty<WorldPosition>("BlockStartPosition", gameObject.Position.ToBlockPosition()),
            gameObject.GetOrAddCustomProperty<int>("BlockRadius", 10));
    }

    public void Update(GameTime gameTime)
    {
        wanderAI.Update(gameTime);
    }
}
