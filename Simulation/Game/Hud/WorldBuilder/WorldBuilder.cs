using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.Hud.WorldBuilder
{
    public enum PlacementMode
    {
        NoPlacement,
        BlockPlacement,
        AmbientObjectPlacement,
        AmbientHitableObjectPlacement,
        LivingEntityPlacement
    }

    public class WorldBuilder
    {
        public static readonly Point TileSize = new Point(32, 32);

        private string[] tilesets = new string[]
        {
            @"Tiles\Exterior01"
        };

        private Rect[] tilesetsPosition;

        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);
        
        private SpriteFont font;
        private bool leftMouseBtnDown;

        private string selectedTileset = null;
        private Point selectedTilesetOffset = Point.Zero;
        private Point? selectedTile = null;
        private Point selectedTileSize = TileSize;

        private Rect tilesetSelectionArea = new Rect(SimulationGame.Resolution.Width * 2 / 3, 0, SimulationGame.Resolution.Width / 3, SimulationGame.Resolution.Height);

        private PlacementMode currentMode = PlacementMode.NoPlacement;

        public void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            font = SimulationGame.ContentManager.Load<SpriteFont>("ArialBig");
            tilesetsPosition = new Rect[tilesets.Length];

            for (int i = 0; i < tilesets.Length; i++)
            {
                var fontSize = font.MeasureString(tilesets[i]);

                tilesetsPosition[i] = new Rect((int)(SimulationGame.Resolution.Width - fontSize.X - 20), 20 + i * 24, (int)fontSize.X, (int)fontSize.Y);
            }
        }

        public void Update(GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                if(currentMode == PlacementMode.NoPlacement)
                {
                    
                }

                MouseState mouseState = Mouse.GetState();

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var mousePosition = Mouse.GetState().Position;

                    if (!leftMouseBtnDown)
                    {
                        leftMouseBtnDown = true;
                        
                        if (selectedTileset == null)
                        {                            
                            for (int i = 0; i < tilesetsPosition.Length; i++)
                            {
                                var tilesetPosition = tilesetsPosition[i];

                                if (tilesetPosition.Contains(mousePosition))
                                {
                                    selectedTileset = tilesets[i];
                                    selectedTilesetOffset = Point.Zero;
                                    selectedTile = null;
                                    selectedTileSize = TileSize;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if(tilesetSelectionArea.Contains(mousePosition))
                            {
                                var relativeMousePosition = new Point(mousePosition.X - tilesetSelectionArea.X + selectedTilesetOffset.X, mousePosition.Y - tilesetSelectionArea.Y + selectedTilesetOffset.Y);

                                selectedTile = GeometryUtils.GetChunkPosition(relativeMousePosition.X, relativeMousePosition.Y, TileSize.X, TileSize.Y);
                                selectedTileSize = TileSize;
                            }
                            else
                            {
                                // Place block
                                var dialog = new InputDialog("Title", JToken.FromObject(BlockType.lookup[0], SerializationUtils.Serializer).ToString(Newtonsoft.Json.Formatting.Indented));

                                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    string result_text = dialog.ResultText;
                                    // use result_text...
                                }
                                else
                                {
                                    // user cancelled out, do something...
                                }
                            }
                        }
                    }
                    else if(selectedTile != null)
                    {
                        var selectedTilePoint = selectedTile ?? Point.Zero;
                        var relativeMousePosition = new Point(mousePosition.X - tilesetSelectionArea.X + selectedTilesetOffset.X, mousePosition.Y - tilesetSelectionArea.Y + selectedTilesetOffset.Y);
                        var relativeMousePositionBlock = GeometryUtils.GetChunkPosition(relativeMousePosition.X, relativeMousePosition.Y, TileSize.X, TileSize.Y);

                        if(relativeMousePositionBlock.X >= selectedTilePoint.X && relativeMousePositionBlock.Y >= selectedTilePoint.Y)
                        {
                            selectedTileSize = new Point(TileSize.X + TileSize.X * (relativeMousePositionBlock.X - selectedTilePoint.X), TileSize.Y + TileSize.Y * (relativeMousePositionBlock.Y - selectedTilePoint.Y));
                        }
                    }
                }
                else
                {
                    leftMouseBtnDown = false;
                }

                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    selectedTilesetOffset.X = Math.Max(0, selectedTilesetOffset.X + 5);
                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    selectedTilesetOffset.X = Math.Max(0, selectedTilesetOffset.X - 5);
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    selectedTilesetOffset.Y = Math.Max(0, selectedTilesetOffset.Y + 5);
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    selectedTilesetOffset.Y = Math.Max(0, selectedTilesetOffset.Y - 5);

                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
                    selectedTileset = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);

                if(selectedTileset == null)
                {
                    for (int i = 0; i < tilesets.Length; i++)
                        spriteBatch.DrawString(font, tilesets[i], new Vector2(tilesetsPosition[i].X, tilesetsPosition[i].Y), Color.White);
                }
                else
                {
                    var texture = SimulationGame.ContentManager.Load<Texture2D>(selectedTileset);
                    var width = Math.Max(0, Math.Min(SimulationGame.Resolution.Width / 3, texture.Width - selectedTilesetOffset.X));
                    var height = Math.Max(0, Math.Min(SimulationGame.Resolution.Height, texture.Height - selectedTilesetOffset.Y));
                    var spritePosition = new Rectangle(selectedTilesetOffset.X, selectedTilesetOffset.Y, width, height);

                    if(spritePosition.Width > 0 && spritePosition.Height > 0)
                    {
                        spriteBatch.Draw(texture,
                        new Vector2(tilesetSelectionArea.X, tilesetSelectionArea.Y), spritePosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                        if (selectedTile != null)
                        {
                            Point selectedTilePoint = selectedTile ?? Point.Zero;
                            Point realTilePosition = new Point(selectedTilePoint.X * TileSize.X, selectedTilePoint.Y * TileSize.Y);

                            if(spritePosition.Contains(realTilePosition))
                            {
                                SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle(tilesetSelectionArea.X + realTilePosition.X - selectedTilesetOffset.X, tilesetSelectionArea.Y + realTilePosition.Y - selectedTilesetOffset.Y, selectedTileSize.X, selectedTileSize.Y), Color.Red);
                            }
                        }
                    }
                }
            }
        }
    }
}
