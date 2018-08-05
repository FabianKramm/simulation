using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Util.UI
{
    public class KeyPressHandler
    {
        private bool keyPressed = false;
        private Keys key;
        private Action callback;

        public KeyPressHandler(Keys key, Action callback)
        {
            this.key = key;
            this.callback = callback;
        }

        public void Update(GameTime gameTime)
        {
            if (SimulationGame.KeyboardState.IsKeyDown(key))
            {
                if(keyPressed == false)
                {
                    callback();
                }

                keyPressed = true;
            }
            else
            {
                keyPressed = false;
            }
        }
    }
}
