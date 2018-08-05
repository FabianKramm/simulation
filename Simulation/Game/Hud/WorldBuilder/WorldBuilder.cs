using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
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

        private Button editBtn;
        private Button createBtn;
        private Button removeBtn;

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
            tilesetSelectionArea = new Rect(ClickBounds.X, ClickBounds.Y + 120, ClickBounds.Width - 50, ClickBounds.Height);

            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            tileSetSelectionView = new TileSetSelectionView(tilesetSelectionArea);
            tilesetSelectionList = new ScrollableList(tilesetSelectionArea);
            tilesetSelectionList.OnSelect((Point position, UIElement selectedElement) =>
            {
                placementMode = PlacementMode.CreateFromTileset;
                tileSetSelectionView.SetTileSet(((Button)selectedElement).Text);
            });

            manageObjectList = new ScrollableList(tilesetSelectionArea);

            foreach (var tileset in tilesets)
            {
                var button = new Button(tileset, Point.Zero);

                button.ShowBorder = false;

                tilesetSelectionList.AddElement(button);
            }

            // Block Type Button
            blockTypeBtn = new Button("Blocks", new Point(ClickBounds.X + 10, ClickBounds.Y + 10));
            blockTypeBtn.OnClick((Point position) => {
                if(placementType != PlacementType.BlockPlacement)
                {
                    placementType = PlacementType.BlockPlacement;
                }
            });

            // Manage Button
            manageBtn = new Button("Manage", new Point(ClickBounds.X + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            manageBtn.OnClick((Point position) => {
                placementMode = PlacementMode.Manage;
                manageObjectList.Clear();

                switch(placementType)
                {
                    case PlacementType.BlockPlacement:
                        foreach (var blockTypeItem in BlockType.lookup)
                            manageObjectList.AddElement(new BlockListItem(blockTypeItem.Value));
                        break;
                }
            });

            // Create From Json
            createFromJsonBtn = new Button("Create From Json", new Point(manageBtn.ClickBounds.Right + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            createFromJsonBtn.OnClick(createNewObject);

            // Create From Tileset
            createFromTilesetBtn = new Button("Create From Tileset", new Point(createFromJsonBtn.ClickBounds.Right + 10, blockTypeBtn.ClickBounds.Bottom + 10));
            createFromTilesetBtn.OnClick((Point position) => placementMode = PlacementMode.ChooseTileset);

            // Edit Btn
            editBtn = new Button("Edit", new Point(ClickBounds.X + 10, manageBtn.ClickBounds.Bottom + 10));
            editBtn.OnClick(editObject);

            // Remove Btn
            removeBtn = new Button("Remove", new Point(editBtn.ClickBounds.Right + 10, manageBtn.ClickBounds.Bottom + 10));
            removeBtn.OnClick(removeObject);

            // Create Btn
            createBtn = new Button("Create", new Point(ClickBounds.X + 10, manageBtn.ClickBounds.Bottom + 10));
            createBtn.OnClick(createNewObject);

            AddElement(blockTypeBtn);
        }

        private void removeObject(Point position)
        {
            var confirmResult = System.Windows.Forms.MessageBox.Show("Are you sure to delete this item?", "Confirm Delete!", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (confirmResult == System.Windows.Forms.DialogResult.Yes)
            {
                switch(placementType)
                {
                    case PlacementType.BlockPlacement:
                        BlockType.lookup.Remove(((BlockListItem)manageObjectList.SelectedElement).BlockType.ID);
                        manageObjectList.RemoveElement(manageObjectList.SelectedElement);
                        break;
                }
            }
        }

        private void editObject(Point position)
        {
            var selectedElement = manageObjectList.SelectedElement;
            object selectedObject = null;

            switch(placementType)
            {
                case PlacementType.BlockPlacement:
                    selectedObject = ((BlockListItem)selectedElement).BlockType;
                    break;
            }

            var dialog = new InputDialog("Edit Object", JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString(Newtonsoft.Json.Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                replaceTypeFromString(dialog.ResultText);
            }
        }

        private void replaceTypeFromString(string objectText)
        {
            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    BlockType newBlockType = JsonConvert.DeserializeObject<BlockType>(objectText, SerializationUtils.SerializerSettings);
                    BlockType.lookup[newBlockType.ID] = newBlockType;
                    break;
            }
        }

        private int generateNewId()
        {
            int highestNumber = int.MinValue;

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    foreach(var blockTypeItem in BlockType.lookup)
                        if (blockTypeItem.Value.ID > highestNumber)
                            highestNumber = blockTypeItem.Value.ID;
                    break;
            }

            return highestNumber + 1;
        }

        private void createNewObject(Point position)
        {
            string spritePath = null;
            Point spritePosition = Point.Zero;
            Point spriteBounds = Point.Zero;

            if(placementMode == PlacementMode.CreateFromTileset)
            {
                spritePath = tileSetSelectionView.SelectedSpritePath;
                spritePosition = tileSetSelectionView.SelectedSpritePosition ?? Point.Zero;
                spriteBounds = tileSetSelectionView.SelectedSpriteBounds;
            }

            object selectedObject = null;
            int newId = generateNewId();

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    selectedObject = new BlockType()
                    {
                        ID=newId,
                        Name="Block"+newId,
                        SpritePath =spritePath,
                        SpritePostion=spritePosition,
                        SpriteBounds=spriteBounds,
                    };
                    break;
            }

            var dialog = new InputDialog("Create Object", JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                replaceTypeFromString(dialog.ResultText);
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

                            if (manageObjectList.SelectedElement != null)
                            {
                                editBtn.Update(gameTime);
                                removeBtn.Update(gameTime);
                            }
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Update(gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Update(gameTime);

                            if (tileSetSelectionView.SelectedSpritePosition != null)
                                createBtn.Update(gameTime);
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

                            if (manageObjectList.SelectedElement != null)
                            {
                                editBtn.Draw(spriteBatch, gameTime);
                                removeBtn.Draw(spriteBatch, gameTime);
                            }   
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Draw(spriteBatch, gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Draw(spriteBatch, gameTime);

                            if (tileSetSelectionView.SelectedSpritePosition != null)
                                createBtn.Draw(spriteBatch, gameTime);
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
