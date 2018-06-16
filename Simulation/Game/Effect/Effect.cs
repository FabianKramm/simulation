using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Effect
{
    abstract public class Effect
    {
        public LivingEntity origin
        {
            get; private set;
        }

        public bool isFinished
        {
            get; protected set;
        }

        public Effect(LivingEntity origin)
        {
            this.origin = origin;
            isFinished = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
