using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Renderer.Effects
{
    public class EffectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, Simulation.Game.Effects.Effect effect)
        {
            if(effect is Slash)
            {
                SlashRenderer.Draw(spriteBatch, gameTime, (Slash)effect);
            }
        }
    }
}
