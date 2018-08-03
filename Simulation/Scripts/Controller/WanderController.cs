using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Interfaces;
using System;

class WanderController: GameObjectController
{
    public GameObjectController Create(GameObject gameObject)
    {
        return new WanderController();
    }

    public void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }
}
