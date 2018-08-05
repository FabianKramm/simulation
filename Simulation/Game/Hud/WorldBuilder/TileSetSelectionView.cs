using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using System;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class TileSetSelectionView: UIElement
    {
        public static readonly Point TileSize = new Point(32, 32);

        private string selectedTileset;
        private Point selectedTilesetOffset = Point.Zero;
        private Point? selectedTile = null;
        private Point selectedTileSize = TileSize;

        private Action<string, Rect> tileSetSelected;

        public TileSetSelectionView(Rect clickBounds)
        {
            ClickBounds = clickBounds;

            OnMouseMove(handleOnMouseMove);
            OnClick(handleOnClick);

            OnKeyPress(Keys.Left, () => selectedTilesetOffset.X = Math.Max(0, selectedTilesetOffset.X + 5));
            OnKeyPress(Keys.Right, () => selectedTilesetOffset.X = Math.Max(0, selectedTilesetOffset.X - 5));
            OnKeyPress(Keys.Up, () => selectedTilesetOffset.Y = Math.Max(0, selectedTilesetOffset.Y + 5));
            OnKeyPress(Keys.Down, () => selectedTilesetOffset.Y = Math.Max(0, selectedTilesetOffset.Y - 5));
        }

        public void SetTileSet(string tileSet, Action<string, Rect> tileSetSelected)
        {
            this.selectedTileset = tileSet;
            this.tileSetSelected = tileSetSelected;   
        }

        private void handleOnClick(Point mousePosition)
        {
            var relativeMousePosition = new Point(mousePosition.X - ClickBounds.X + selectedTilesetOffset.X, mousePosition.Y - ClickBounds.Y + selectedTilesetOffset.Y);

            selectedTile = GeometryUtils.GetChunkPosition(relativeMousePosition.X, relativeMousePosition.Y, TileSize.X, TileSize.Y);
            selectedTileSize = TileSize;
        }

        private void handleOnMouseMove(MouseMoveEvent mouseMoveEvent)
        {
            if (mouseMoveEvent.LeftButtonDown)
            {
                var selectedTilePoint = selectedTile ?? Point.Zero;
                var relativeMousePosition = new Point(mouseMoveEvent.MousePosition.X - ClickBounds.X + selectedTilesetOffset.X, mouseMoveEvent.MousePosition.Y - ClickBounds.Y + selectedTilesetOffset.Y);
                var relativeMousePositionBlock = GeometryUtils.GetChunkPosition(relativeMousePosition.X, relativeMousePosition.Y, TileSize.X, TileSize.Y);

                if (relativeMousePositionBlock.X >= selectedTilePoint.X && relativeMousePositionBlock.Y >= selectedTilePoint.Y)
                {
                    selectedTileSize = new Point(TileSize.X + TileSize.X * (relativeMousePositionBlock.X - selectedTilePoint.X), TileSize.Y + TileSize.Y * (relativeMousePositionBlock.Y - selectedTilePoint.Y));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(selectedTileset != null)
            {
                var texture = SimulationGame.ContentManager.Load<Texture2D>(selectedTileset);
                var width = Math.Max(0, Math.Min(SimulationGame.Resolution.Width / 3, texture.Width - selectedTilesetOffset.X));
                var height = Math.Max(0, Math.Min(SimulationGame.Resolution.Height, texture.Height - selectedTilesetOffset.Y));
                var spritePosition = new Rectangle(selectedTilesetOffset.X, selectedTilesetOffset.Y, width, height);

                if (spritePosition.Width > 0 && spritePosition.Height > 0)
                {
                    spriteBatch.Draw(texture,
                    new Vector2(ClickBounds.X, ClickBounds.Y), spritePosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                    if (selectedTile != null)
                    {
                        Point selectedTilePoint = selectedTile ?? Point.Zero;
                        Point realTilePosition = new Point(selectedTilePoint.X * TileSize.X, selectedTilePoint.Y * TileSize.Y);

                        if (spritePosition.Contains(realTilePosition))
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(ClickBounds.X + realTilePosition.X - selectedTilesetOffset.X, ClickBounds.Y + realTilePosition.Y - selectedTilesetOffset.Y, selectedTileSize.X, selectedTileSize.Y), Color.Red);
                        }
                    }
                }
            }
        }
    }
}
