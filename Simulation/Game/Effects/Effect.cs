using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;

namespace Simulation.Game.Effects
{
    abstract public class Effect
    {
        public string ID
        {
            get; private set;
        }

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

            ID = Util.Util.getUUID();
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
