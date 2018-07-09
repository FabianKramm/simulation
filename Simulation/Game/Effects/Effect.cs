using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base.Entity;
using Simulation.Game.Renderer.Effects;

namespace Simulation.Game.Effects
{
    abstract public class Effect
    {
        public EffectRendererInformation effectRendererInformation;

        public string ID
        {
            get; private set;
        }

        public LivingEntity origin
        {
            get; private set;
        }

        public bool IsFinished
        {
            get; protected set;
        }

        public Effect(LivingEntity origin)
        {
            this.origin = origin;
            IsFinished = false;

            ID = Util.Util.getUUID();
        }

        public abstract void Update(GameTime gameTime);
    }
}
