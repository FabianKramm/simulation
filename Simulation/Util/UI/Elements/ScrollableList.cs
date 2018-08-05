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
            Bounds = listBounds;

            previousScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;
            
            OnKeyHold(Keys.Up, handleKeyUp, TimeSpan.FromMilliseconds(200));
            OnKeyHold(Keys.Down, handleKeyDown, TimeSpan.FromMilliseconds(200));
        }

        private void handleKeyUp()
        {
            var currentIndex = 0;

            if (SelectedElement != null)
                currentIndex = elements.IndexOf(SelectedElement);

            currentIndex = Math.Max(0, currentIndex - 1);
            SelectedElement = elements[currentIndex];
        }

        private void handleKeyDown()
        {
            var currentIndex = 0;

            if (SelectedElement != null)
                currentIndex = elements.IndexOf(SelectedElement);

            currentIndex = Math.Min(elements.Count - 1, currentIndex + 1);
            SelectedElement = elements[currentIndex];
        }

        public void Clear()
        {
            SelectedElement = null;

            elements.Clear();
        }

        public void ScrollToElement(UIElement element)
        {
            var index = elements.IndexOf(element);

            if (index >= 0)
            {
                int top = 0;

                foreach (var _element in elements)
                {
                    if (_element == element)
                    {
                        relativeTop = -top;
                    }

                    top += _element.Bounds.Height;
                }
            }
        }

        public void SelectElement(UIElement element)
        {
            if (element == null)
                return;

            var index = elements.IndexOf(element);

            if(index >= 0)
            {
                SelectedElement = element;
                ScrollToElement(element);
            }
        }

        public UIElement[] GetElements()
        {
            return elements.ToArray();
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
            element.Bounds.Y = Bounds.Y;
            element.Bounds.X = Bounds.X;
            element.Bounds.Width = Bounds.Width;

            element.OnClick((Point position) =>
            {
                handleElementClick(position, element);
            });

            elements.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            if (element == SelectedElement)
                SelectedElement = null;

            elements.Remove(element);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(IsScrollable)
            {
                var newScrollWheelValue = SimulationGame.MouseState.ScrollWheelValue;

                if (newScrollWheelValue < previousScrollWheelValue)
                {
                    relativeTop = relativeTop - 5;
                }
                else if (newScrollWheelValue > previousScrollWheelValue)
                {
                    relativeTop = Math.Min(0, relativeTop + 5);
                }

                previousScrollWheelValue = newScrollWheelValue;
            }

            int top = Bounds.Y + relativeTop;

            foreach(var element in elements)
            {
                element.Bounds.Y = top;
                top += element.Bounds.Height;

                element.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var element in elements)
                if (Bounds.Contains(element.Bounds.X, element.Bounds.Y))
                    element.Draw(spriteBatch, gameTime);

            if(SelectedElement != null)
                if(Bounds.Contains(SelectedElement.Bounds.X, SelectedElement.Bounds.Y))
                {
                    SimulationGame.PrimitiveDrawer.Rectangle(SelectedElement.Bounds.ToXnaRectangle(), Color.Orange);
                }
        }
    }
}
