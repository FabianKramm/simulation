using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.Util.UI
{
    public struct MouseMoveEvent
    {
        public Point MousePosition;
        public bool LeftButtonDown;
    }

    public abstract class UIElement
    {
        private List<KeyPressHandler> keyPressHandler = new List<KeyPressHandler>();
        private bool leftMouseButtonDown = false;
        private Action<Point> onClickHandler;
        private Action<MouseMoveEvent> onMouseMoveHandler;
        private Point lastMousePosition;

        public bool IsHover
        {
            get; private set;
        }

        public Rect ClickBounds;

        public void OnKeyPress(Keys key, Action callback)
        {
            keyPressHandler.Add(new KeyPressHandler(key, callback));
        }

        public void OnMouseMove(Action<MouseMoveEvent> callback)
        {
            onMouseMoveHandler = callback;
        }

        public void OnClick(Action<Point> callback)
        {
            onClickHandler = callback;
        }

        public virtual void Update(GameTime gameTime)
        {
            var mouseState = SimulationGame.MouseState;

            IsHover = ClickBounds.Contains(mouseState.Position);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if(leftMouseButtonDown == false && IsHover)
                {
                    onClickHandler?.Invoke(mouseState.Position);
                }

                leftMouseButtonDown = true;
            }
            else
            {
                leftMouseButtonDown = false;
            }

            foreach (var handler in keyPressHandler)
                handler.Update(gameTime);

            if(onMouseMoveHandler != null)
            {
                if(lastMousePosition != mouseState.Position && ClickBounds.Contains(mouseState.Position))
                {
                    onMouseMoveHandler(new MouseMoveEvent
                    {
                        MousePosition = mouseState.Position,
                        LeftButtonDown = mouseState.LeftButton == ButtonState.Pressed
                    });
                }
            }
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
