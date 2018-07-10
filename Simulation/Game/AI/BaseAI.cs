using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI
{
    public abstract class BaseAI
    {
        public MovingEntity Entity
        {
            get; private set;
        }

        public BaseAI(MovingEntity movingEntity)
        {
            Entity = movingEntity;
        }

        public abstract void Update(GameTime gameTime);
    }
}
