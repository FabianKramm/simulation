using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util.Geometry;

namespace Simulation.Util.UI.Elements
{
    public class ScrollableList: UIElement
    {
        private List<UIElement> elements = new List<UIElement>();
        private int relativeTop = 0;
        private int previousScrollWheelValue = 0;

        public ScrollableList(Rect listBounds)
        {
            ClickBounds = listBounds;

            previousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public void AddElement(UIElement element)
        {
            element.ClickBounds.Y = 0;
            element.ClickBounds.X = ClickBounds.X + element.ClickBounds.X;

            elements.Add(element);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var newScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;

            if(newScrollWheelValue < previousScrollWheelValue)
            {
                relativeTop = Math.Max(0, relativeTop - 2);
            }
            else if(newScrollWheelValue > previousScrollWheelValue)
            {
                relativeTop = relativeTop + 2;
            }

            previousScrollWheelValue = newScrollWheelValue;

            int top = relativeTop;

            foreach(var element in elements)
            {
                element.ClickBounds.Y = top;
                top += element.ClickBounds.Height;

                element.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var element in elements)
                if (ClickBounds.Contains(element.ClickBounds.X, element.ClickBounds.Y))
                    element.Draw(spriteBatch, gameTime);
        }
    }
}
