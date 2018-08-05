using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Simulation.Util.UI
{
    public class BaseUI: UIElement
    {
        private List<UIElement> uIElements = new List<UIElement>();

        public void AddElement(UIElement element)
        {
            uIElements.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            uIElements.Remove(element);
        }

        public UIElement[] GetUIElements()
        {
            return uIElements.ToArray();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var uiElement in uIElements)
                uiElement.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var uiElement in uIElements)
                uiElement.Draw(spriteBatch, gameTime);
        }
    }
}
