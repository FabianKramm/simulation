using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;
using System;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class WorldBuilder: BaseUI
    {
        private enum PlacementType
        {
            NoType,
            BlockPlacement,
            AmbientObjectPlacement,
            AmbientHitableObjectPlacement,
            LivingEntityPlacement
        }

        private enum PlacementMode
        {
            NoPlacement,
            Manage,
            ChooseTileset,
            CreateFromTileset,
            CreateFromJson
        }

        private string[] tilesets = new string[]
        {
            @"Tiles\Exterior01"
        };

        private Button changeWorldMapBtn;
        private Button blockTypeBtn;
        private Button ambientObjectTypeBtn;
        private Button ambientHitableObjectTypeBtn;
        private Button livingEntityTypeBtn;

        private Button manageBtn;
        private Button createFromTilesetBtn;
        private Button createFromJsonBtn;

        private TileSetSelectionView tileSetSelectionView;
        private ScrollableList tilesetSelectionList;

        private ScrollableList manageObjectList;

        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);

        private Rect tilesetSelectionArea;

        private PlacementType placementType = PlacementType.NoType;
        private PlacementMode placementMode = PlacementMode.NoPlacement;

        public void LoadContent()
        {
            ClickBounds = new Rect(SimulationGame.Resolution.Width * 2 / 3, 0, SimulationGame.Resolution.Width / 3, SimulationGame.Resolution.Height);
            tilesetSelectionArea = new Rect(ClickBounds.X, ClickBounds.Y + 100, ClickBounds.Width - 50, ClickBounds.Height);

            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            tileSetSelectionView = new TileSetSelectionView(tilesetSelectionArea);
            tilesetSelectionList = new ScrollableList(tilesetSelectionArea);
            tilesetSelectionList.OnSelect((Point position, UIElement selectedElement) =>
            {
                placementMode = PlacementMode.CreateFromTileset;
                tileSetSelectionView.SetTileSet(((Button)selectedElement).Text, OnTileSetSelected);
            });

            manageObjectList = new ScrollableList(tilesetSelectionArea);

            foreach (var tileset in tilesets)
            {
                var button = new Button(tileset, Point.Zero);

                button.ShowBorder = false;

                tilesetSelectionList.AddElement(button);
            }

            OnKeyPress(Keys.Back, () =>
            {
                if(placementMode == PlacementMode.CreateFromTileset)
                {
                    placementMode = PlacementMode.ChooseTileset;
                }
            });

            // Block Type Button
            blockTypeBtn = new Button("Blocks", new Point(ClickBounds.X + 10, ClickBounds.Y + 10));
            blockTypeBtn.OnClick((Point position) => {
                if(placementType != PlacementType.BlockPlacement)
                {
                    placementType = PlacementType.BlockPlacement;
                    manageObjectList.Clear();

                    foreach (var blockTypeItem in BlockType.lookup)
                        manageObjectList.AddElement(new BlockListItem(blockTypeItem.Value));
                }
            });

            // Manage Button
            manageBtn = new Button("Manage", new Point(ClickBounds.X + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            manageBtn.OnClick((Point position) => placementMode = PlacementMode.Manage);

            // Create From Json
            createFromJsonBtn = new Button("Create From Json", new Point(manageBtn.ClickBounds.Right + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            createFromJsonBtn.OnClick((Point position) => createNewObject());

            // Create From Tileset
            createFromTilesetBtn = new Button("Create From Tileset", new Point(createFromJsonBtn.ClickBounds.Right + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            createFromTilesetBtn.OnClick((Point position) => placementMode = PlacementMode.ChooseTileset);

            AddElement(blockTypeBtn);
        }

        private void createNewObject()
        {

        }

        public void OnTileSetSelected(string tileset, Rect spriteBounds)
        {
            // Create bois
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

        public override void Update(GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                base.Update(gameTime);

                if (placementType != PlacementType.NoType)
                {
                    switch (placementMode)
                    {
                        case PlacementMode.Manage:
                            manageObjectList.Update(gameTime);
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Update(gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Update(gameTime);
                            break;
                    }

                    manageBtn.Update(gameTime);
                    createFromJsonBtn.Update(gameTime);

                    if (placementType != PlacementType.LivingEntityPlacement)
                        createFromTilesetBtn.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);
                base.Draw(spriteBatch, gameTime);

                if(placementType != PlacementType.NoType)
                {
                    switch (placementMode)
                    {
                        case PlacementMode.Manage:
                            manageObjectList.Draw(spriteBatch, gameTime);
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Draw(spriteBatch, gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Draw(spriteBatch, gameTime);
                            break;
                    }

                    manageBtn.Draw(spriteBatch, gameTime);
                    createFromJsonBtn.Draw(spriteBatch, gameTime);

                    if(placementType != PlacementType.LivingEntityPlacement)
                        createFromTilesetBtn.Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
