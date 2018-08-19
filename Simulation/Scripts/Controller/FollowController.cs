﻿using Microsoft.Xna.Framework;
using Simulation.Game.AI;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Scripts.Base;

namespace Simulation.Scripts.Controller
{
    public class FollowController : GameObjectController
    {
        private FollowAI followAI;

        public void Init(GameObject gameObject)
        {
            followAI = new FollowAI((MovingEntity)gameObject,
                gameObject.GetOrAddCustomProperty<string>("TargetID", SimulationGame.Player.ID),
                gameObject.GetOrAddCustomProperty<float>("Distance", 50.0f));
        }

        public void Update(GameTime gameTime)
        {
            followAI.Update(gameTime);
        }
    }
}
