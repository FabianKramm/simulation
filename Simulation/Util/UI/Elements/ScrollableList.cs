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
        private Action<Point, UIElement> handleSelectElement;

        public UIElement SelectedElement
        {
            get; private set;
        }

        public bool IsScrollable = true;

        public ScrollableList(Rect listBounds)
        {
            ClickBounds = listBounds;

            previousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public void Clear()
        {
            elements.Clear();
        }

        public void OnSelect(Action<Point, UIElement> callback)
        {
            this.handleSelectElement = callback;
        }

        private void handleElementClick(Point position, UIElement element)
        {
            SelectedElement = element;

            handleSelectElement?.Invoke(position, element);
        }

        public void AddElement(UIElement element)
        {
            element.ClickBounds.Y = ClickBounds.Y;
            element.ClickBounds.X = ClickBounds.X;
            element.ClickBounds.Width = ClickBounds.Width;

            element.OnClick((Point position) =>
            {
                handleElementClick(position, element);
            });

            elements.Add(element);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(IsScrollable)
            {
                var newScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;

                if (newScrollWheelValue < previousScrollWheelValue)
                {
                    relativeTop = Math.Max(0, relativeTop - 5);
                }
                else if (newScrollWheelValue > previousScrollWheelValue)
                {
                    relativeTop = relativeTop + 5;
                }

                previousScrollWheelValue = newScrollWheelValue;
            }

            int top = ClickBounds.Y + relativeTop;

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

            if(SelectedElement != null)
                if(ClickBounds.Contains(SelectedElement.ClickBounds.X, SelectedElement.ClickBounds.Y))
                {
                    SimulationGame.PrimitiveDrawer.Rectangle(SelectedElement.ClickBounds.ToXnaRectangle(), Color.Orange);
                }
        }
    }
}
