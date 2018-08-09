using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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
                keyPressed = true;
            }
            else
            {
                if (keyPressed == true)
                {
                    callback();
                }

                keyPressed = false;
            }
        }
    }
}
