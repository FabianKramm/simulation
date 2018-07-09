using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Hud
{
    public class GameConsole
    {
        private static int consoleLines = 40;
        private static List<string> messages = new List<string>();

        private SpriteFont font;
        private static List<string> blackList = new List<string>();
        private static List<string> whiteList = new List<string>();

        public static void WriteLine(string message)
        {
            lock (messages)
            {
                messages.Add(DateTime.Now.ToString("mm:ss") + ": " + message);

                if (messages.Count >= consoleLines)
                {
                    messages.RemoveAt(0);
                }
            }
        }

        public static void WriteLine(string ID, string message)
        {
            if(whiteList.Count == 0 || whiteList.Contains(ID))
            {
                if(!blackList.Contains(ID))
                {
                    lock(messages)
                    {
                        messages.Add("[" + ID + "] " + DateTime.Now.ToString("mm:ss") + ": " + message);

                        if (messages.Count >= consoleLines)
                        {
                            messages.RemoveAt(0);
                        }
                    }
                }
            }
        }

        public void LoadContent()
        {
            font = SimulationGame.ContentManager.Load<SpriteFont>("Arial");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(SimulationGame.IsDebug)
            {
                lock(messages)
                {
                    for (int i = 0; i < messages.Count; i++)
                    {
                        var message = messages.ElementAt(i);

                        spriteBatch.DrawString(font, message, new Vector2(5, 5 + i * 16), Color.White);
                    }
                }
            }
        }
    }
}
