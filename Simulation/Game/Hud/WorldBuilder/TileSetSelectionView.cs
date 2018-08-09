using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using System;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class TileSetSelectionView : UIElement
    {
        public static readonly Point InitialTileSize = new Point(16, 16);
        public static readonly Point TileSize = new Point(16, 16);

        public string SelectedSpritePath
        {
            get; private set;
        }

        public Point? SelectedSpritePosition = null;
        public Point SelectedSpriteBounds = InitialTileSize;

        private Point scrollOffset = Point.Zero;

        private bool mouseBtnPressed = false;

        public TileSetSelectionView(Rect clickBounds)
        {
            Bounds = clickBounds;

            OnMouseMove(handleOnMouseMove);
            OnKeyHold(Keys.Left, handleLeftKeyDown);
            OnKeyHold(Keys.Right, handleRightKeyDown);
            OnKeyHold(Keys.Up, handleUpKeyDown);
            OnKeyHold(Keys.Down, handleDownKeyDown);
        }

        private void handleDownKeyDown()
        {
            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftShift) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightShift))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X, selectedSpritePositionPoint.Y + 1);
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                SelectedSpriteBounds.Y = SelectedSpriteBounds.Y + 1;
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftAlt) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightAlt))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X, selectedSpritePositionPoint.Y + TileSize.Y);
            }
            else
            {
                scrollOffset.Y = Math.Max(0, scrollOffset.Y - 5);
            }
        }

        private void handleUpKeyDown()
        {
            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftShift) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightShift))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X, Math.Max(0, selectedSpritePositionPoint.Y - 1));
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                SelectedSpriteBounds.Y = Math.Max(0, SelectedSpriteBounds.Y - 1);
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftAlt) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightAlt))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X, Math.Max(0, selectedSpritePositionPoint.Y - TileSize.Y));
            }
            else
            {
                scrollOffset.Y = scrollOffset.Y + 5;
            }
        }

        private void handleRightKeyDown()
        {
            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftShift) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightShift))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X + 1, selectedSpritePositionPoint.Y);
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                SelectedSpriteBounds.X = SelectedSpriteBounds.X + 1;
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftAlt) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightAlt))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(selectedSpritePositionPoint.X + TileSize.X, selectedSpritePositionPoint.Y);
            }
            else
            {
                scrollOffset.X = Math.Max(0, scrollOffset.X - 5);
            }
        }

        private void handleLeftKeyDown()
        {
            if(SimulationGame.KeyboardState.IsKeyDown(Keys.LeftShift) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightShift))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(Math.Max(0, selectedSpritePositionPoint.X - 1), selectedSpritePositionPoint.Y);
            }
            else if(SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                SelectedSpriteBounds.X = Math.Max(0, SelectedSpriteBounds.X - 1);
            }
            else if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftAlt) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightAlt))
            {
                var selectedSpritePositionPoint = SelectedSpritePosition ?? Point.Zero;

                SelectedSpritePosition = new Point(Math.Max(0, selectedSpritePositionPoint.X - TileSize.X), selectedSpritePositionPoint.Y);
            }
            else
            {
                scrollOffset.X = scrollOffset.X + 5;
            }
        }

        public void SetTileSet(string tileSet)
        {
            SelectedSpritePath = tileSet;
        }

        private void handleOnMouseMove(MouseMoveEvent mouseMoveEvent)
        {
            if (mouseMoveEvent.LeftButtonDown)
            {
                if(!mouseBtnPressed)
                {
                    var mousePosition = SimulationGame.MouseState.Position;
                    var relativeMousePosition = new Point(mousePosition.X - Bounds.X + scrollOffset.X, mousePosition.Y - Bounds.Y + scrollOffset.Y);

                    if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        SelectedSpritePosition = new Point(relativeMousePosition.X, relativeMousePosition.Y);
                        SelectedSpriteBounds = InitialTileSize;
                    }
                    else
                    {
                        var tilePosition = GeometryUtils.GetChunkPosition(relativeMousePosition.X, relativeMousePosition.Y, TileSize.X, TileSize.Y);

                        SelectedSpritePosition = new Point(tilePosition.X * TileSize.X, tilePosition.Y * TileSize.Y);
                        SelectedSpriteBounds = InitialTileSize;
                    }
                }

                mouseBtnPressed = true;

                if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                {
                    var selectedTilePoint = SelectedSpritePosition ?? Point.Zero;
                    var relativeMousePosition = new Point(mouseMoveEvent.MousePosition.X - Bounds.X + scrollOffset.X, mouseMoveEvent.MousePosition.Y - Bounds.Y + scrollOffset.Y);

                    if (relativeMousePosition.X >= selectedTilePoint.X && relativeMousePosition.Y >= selectedTilePoint.Y)
                    {
                        SelectedSpriteBounds = new Point(relativeMousePosition.X - selectedTilePoint.X, relativeMousePosition.Y - selectedTilePoint.Y);
                    }
                }
                else
                {
                    var selectedTilePoint = SelectedSpritePosition ?? Point.Zero;
                    var relativeMousePosition = new Point(mouseMoveEvent.MousePosition.X - Bounds.X + scrollOffset.X, mouseMoveEvent.MousePosition.Y - Bounds.Y + scrollOffset.Y);
                    
                    if (relativeMousePosition.X >= selectedTilePoint.X && relativeMousePosition.Y >= selectedTilePoint.Y)
                    {
                        var newWidth = relativeMousePosition.X - selectedTilePoint.X;
                        var newHeight = relativeMousePosition.Y - selectedTilePoint.Y;

                        var moduloX = newWidth % TileSize.X != 0 ? TileSize.X - newWidth % TileSize.X : 0;
                        var moduloY = newHeight % TileSize.Y != 0 ? TileSize.Y - newHeight % TileSize.Y : 0;

                        SelectedSpriteBounds = new Point(newWidth + moduloX, newHeight + moduloY);
                    }
                }
            }
            else
            {
                mouseBtnPressed = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SelectedSpritePath != null)
            {
                var texture = SimulationGame.ContentManager.Load<Texture2D>(SelectedSpritePath);
                var width = Math.Max(0, Math.Min(texture.Width, texture.Width - scrollOffset.X));
                var height = Math.Max(0, Math.Min(texture.Height, texture.Height - scrollOffset.Y));
                var spritePosition = new Rectangle(scrollOffset.X, scrollOffset.Y, width, height);

                if (spritePosition.Width > 0 && spritePosition.Height > 0)
                {
                    spriteBatch.Draw(texture,
                    new Vector2(Bounds.X, Bounds.Y), spritePosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                    if (SelectedSpritePosition != null)
                    {
                        Point selectedTilePoint = SelectedSpritePosition ?? Point.Zero;
                        
                        if (spritePosition.Contains(selectedTilePoint))
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(Bounds.X + selectedTilePoint.X - scrollOffset.X, Bounds.Y + selectedTilePoint.Y - scrollOffset.Y, SelectedSpriteBounds.X, SelectedSpriteBounds.Y), Color.Red);
                        }
                    }
                }
            }
        }
    }
}
