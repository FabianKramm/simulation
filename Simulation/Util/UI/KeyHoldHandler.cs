using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Util.UI
{
    public class KeyHoldHandler
    {
        private Keys key;
        private Action callback;
        private TimeSpan timeout;

        private TimeSpan callbackTimeout = TimeSpan.Zero;

        public KeyHoldHandler(Keys key, Action callback, TimeSpan? timeout = null)
        {
            this.timeout = timeout ?? TimeSpan.FromMilliseconds(100);
            this.key = key;
            this.callback = callback;
        }

        public void Update(GameTime gameTime)
        {
            if(SimulationGame.KeyboardState.IsKeyDown(key))
            {
                callbackTimeout += gameTime.ElapsedGameTime;

                if(callbackTimeout > timeout)
                {
                    callback();
                    callbackTimeout = TimeSpan.Zero;
                }
            }
        }
    }
}
