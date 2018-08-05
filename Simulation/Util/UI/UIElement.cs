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
        private List<KeyHoldHandler> keyHoldHandler = new List<KeyHoldHandler>();

        private bool leftMouseButtonDown = false;
        private Action onClickHandler;
        private Action<MouseMoveEvent> onMouseMoveHandler;
        private Point lastMousePosition;

        public bool IsHover
        {
            get; private set;
        }

        public Rect Bounds;

        public void OnKeyHold(Keys key, Action callback, TimeSpan? tickTimeout = null)
        {
            keyHoldHandler.Add(new KeyHoldHandler(key, callback, tickTimeout));
        }

        public void OnKeyPress(Keys key, Action callback)
        {
            keyPressHandler.Add(new KeyPressHandler(key, callback));
        }

        public void OnMouseMove(Action<MouseMoveEvent> callback)
        {
            onMouseMoveHandler = callback;
        }

        public void OnClick(Action callback)
        {
            onClickHandler = callback;
        }

        public void OffClick()
        {
            onClickHandler = null;
        }

        public virtual void Update(GameTime gameTime)
        {
            var mouseState = SimulationGame.MouseState;

            IsHover = Bounds.Contains(mouseState.Position);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if(leftMouseButtonDown == false && IsHover)
                {
                    onClickHandler?.Invoke();
                }

                leftMouseButtonDown = true;
            }
            else
            {
                leftMouseButtonDown = false;
            }

            foreach (var handler in keyPressHandler)
                handler.Update(gameTime);

            foreach (var handler in keyHoldHandler)
                handler.Update(gameTime);

            if (onMouseMoveHandler != null)
            {
                if(lastMousePosition != mouseState.Position && Bounds.Contains(mouseState.Position))
                {
                    onMouseMoveHandler(new MouseMoveEvent
                    {
                        MousePosition = mouseState.Position,
                        LeftButtonDown = mouseState.LeftButton == ButtonState.Pressed
                    });

                    lastMousePosition = mouseState.Position;
                }
            }
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
