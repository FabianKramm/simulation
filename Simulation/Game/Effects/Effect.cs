using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;

namespace Simulation.Game.Effects
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
