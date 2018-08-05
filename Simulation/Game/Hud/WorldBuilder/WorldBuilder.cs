using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class WorldBuilder: BaseUI
    {
        public enum PlacementType
        {
            NoType,
            Inspect,
            BlockPlacement,
            AmbientObjectPlacement,
            AmbientHitableObjectPlacement,
            LivingEntityPlacement
        }

        public enum PlacementMode
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

        private Button inspectBtn;
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

        private InspectView inspectView;
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
            Bounds = new Rect(SimulationGame.Resolution.Width * 2 / 3, 0, SimulationGame.Resolution.Width / 3, SimulationGame.Resolution.Height);
            tilesetSelectionArea = new Rect(Bounds.X, Bounds.Y + 120, Bounds.Width - 50, Bounds.Height);

            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            inspectView = new InspectView(new Rect(0, 0, SimulationGame.Resolution.Width * 2 / 3, SimulationGame.Resolution.Height));

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

            // Inspect Button
            inspectBtn = new Button("Inspect", new Point(Bounds.X + 10, Bounds.Y + 10));
            inspectBtn.OnClick((Point position) =>
            {
                placementType = PlacementType.Inspect;
                placementMode = PlacementMode.NoPlacement;
            });

            // Block Type Button
            blockTypeBtn = new Button("Blocks", new Point(inspectBtn.Bounds.Right + 10, Bounds.Y + 10));
            blockTypeBtn.OnClick((Point position) => {
                placementType = PlacementType.BlockPlacement;
                handleManageBtnClick(Point.Zero);
            });

            // Ambient Object Type Button
            ambientObjectTypeBtn = new Button("Ambient Objects", new Point(blockTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            ambientObjectTypeBtn.OnClick((Point position) => {
                placementType = PlacementType.AmbientObjectPlacement;
                handleManageBtnClick(Point.Zero);
            });

            // Ambient Hitable Object Type Button
            ambientHitableObjectTypeBtn = new Button("Hitable Objects", new Point(ambientObjectTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            ambientHitableObjectTypeBtn.OnClick((Point position) => {
                placementType = PlacementType.AmbientHitableObjectPlacement;
                handleManageBtnClick(Point.Zero);
            });

            // Ambient Hitable Object Type Button
            livingEntityTypeBtn = new Button("Living Entities", new Point(ambientHitableObjectTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            livingEntityTypeBtn.OnClick((Point position) => {
                placementType = PlacementType.LivingEntityPlacement;
                handleManageBtnClick(Point.Zero);
            });

            // Manage Button
            manageBtn = new Button("Manage", new Point(Bounds.X + 10, blockTypeBtn.Bounds.Bottom + 10));
            manageBtn.OnClick(handleManageBtnClick);

            // Create From Json
            createFromJsonBtn = new Button("Create From Json", new Point(manageBtn.Bounds.Right + 10, blockTypeBtn.Bounds.Bottom + 10));
            createFromJsonBtn.OnClick(createNewObject);

            // Create From Tileset
            createFromTilesetBtn = new Button("Create From Tileset", new Point(createFromJsonBtn.Bounds.Right + 10, blockTypeBtn.Bounds.Bottom + 10));
            createFromTilesetBtn.OnClick((Point position) => placementMode = PlacementMode.ChooseTileset);

            // Edit Btn
            editBtn = new Button("Edit", new Point(Bounds.X + 10, manageBtn.Bounds.Bottom + 10));
            editBtn.OnClick(editObject);

            // Remove Btn
            removeBtn = new Button("Remove", new Point(editBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            removeBtn.OnClick(removeObject);

            // Create Btn
            createBtn = new Button("Create", new Point(Bounds.X + 10, manageBtn.Bounds.Bottom + 10));
            createBtn.OnClick(createNewObject);

            AddElement(inspectBtn);
            AddElement(blockTypeBtn);
            AddElement(ambientObjectTypeBtn);
            AddElement(ambientHitableObjectTypeBtn);
            AddElement(livingEntityTypeBtn);
        }

        private void handleManageBtnClick(Point position)
        {
            placementMode = PlacementMode.Manage;
            manageObjectList.Clear();

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    foreach (var blockTypeItem in BlockType.lookup)
                        manageObjectList.AddElement(new BlockListItem(blockTypeItem.Value));
                    break;
                case PlacementType.AmbientObjectPlacement:
                    foreach (var item in AmbientObjectType.lookup)
                        manageObjectList.AddElement(new AmbientObjectListItem(item.Value));
                    break;
                case PlacementType.AmbientHitableObjectPlacement:
                    foreach (var item in AmbientHitableObjectType.lookup)
                        manageObjectList.AddElement(new AmbientHitableObjectListItem(item.Value));
                    break;
                case PlacementType.LivingEntityPlacement:
                    foreach (var item in LivingEntityType.lookup)
                        manageObjectList.AddElement(new LivingEntityListItem(item.Value));
                    break;
            }
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
                    case PlacementType.AmbientObjectPlacement:
                        AmbientObjectType.lookup.Remove(((AmbientObjectListItem)manageObjectList.SelectedElement).AmbientObjectType.ID);
                        manageObjectList.RemoveElement(manageObjectList.SelectedElement);
                        break;
                    case PlacementType.AmbientHitableObjectPlacement:
                        AmbientHitableObjectType.lookup.Remove(((AmbientHitableObjectListItem)manageObjectList.SelectedElement).AmbientHitableObjectType.ID);
                        manageObjectList.RemoveElement(manageObjectList.SelectedElement);
                        break;
                    case PlacementType.LivingEntityPlacement:
                        LivingEntityType.lookup.Remove(((LivingEntityListItem)manageObjectList.SelectedElement).LivingEntityType.ID);
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
                case PlacementType.AmbientObjectPlacement:
                    selectedObject = ((AmbientObjectListItem)selectedElement).AmbientObjectType;
                    break;
                case PlacementType.AmbientHitableObjectPlacement:
                    selectedObject = ((AmbientHitableObjectListItem)selectedElement).AmbientHitableObjectType;
                    break;
                case PlacementType.LivingEntityPlacement:
                    selectedObject = ((LivingEntityListItem)selectedElement).LivingEntityType;
                    break;
            }

            var dialog = new InputDialog("Edit Object", JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WorldBuilderUtils.ReplaceTypeFromString(placementType, dialog.ResultText);
            }
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
            int newId = WorldBuilderUtils.GenerateNewId(placementType);

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    selectedObject = new BlockType()
                    {
                        ID=newId,
                        Name="Block"+newId,
                        SpritePath=spritePath,
                        SpritePosition=spritePosition,
                        SpriteBounds=spriteBounds,
                    };
                    break;
                case PlacementType.AmbientObjectPlacement:
                    selectedObject = new AmbientObjectType()
                    {
                        ID = newId,
                        Name = "AmbientObj" + newId,
                        SpritePath = spritePath,
                        SpritePositions = new Point[] { spritePosition },
                        SpriteBounds = spriteBounds,
                    };
                    break;
                case PlacementType.AmbientHitableObjectPlacement:
                    selectedObject = new AmbientHitableObjectType()
                    {
                        ID = newId,
                        Name = "HitableObj" + newId,
                        SpritePath = spritePath,
                        SpritePositions = new Point[] { spritePosition },
                        SpriteBounds = spriteBounds,
                    };
                    break;
                case PlacementType.LivingEntityPlacement:
                    selectedObject = new LivingEntityType()
                    {
                        ID = newId,
                        Name = "LivingEntity" + newId
                    };
                    break;
            }

            var dialog = new InputDialog("Create Object", JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WorldBuilderUtils.ReplaceTypeFromString(placementType, dialog.ResultText);

                if (placementMode == PlacementMode.CreateFromTileset)
                {
                    placementMode = PlacementMode.Manage;
                    manageObjectList.Clear();

                    UIElement selectedItem = null;

                    switch (placementType)
                    {
                        case PlacementType.BlockPlacement:
                            foreach (var item in BlockType.lookup)
                            {
                                var newItem = new BlockListItem(item.Value);

                                if (item.Value.ID == newId)
                                    selectedItem = newItem;

                                manageObjectList.AddElement(newItem);
                            }
                                
                            break;
                        case PlacementType.AmbientObjectPlacement:
                            foreach (var item in AmbientObjectType.lookup)
                            {
                                var newItem = new AmbientObjectListItem(item.Value);

                                if (item.Value.ID == newId)
                                    selectedItem = newItem;

                                manageObjectList.AddElement(newItem);
                            }

                            break;
                        case PlacementType.AmbientHitableObjectPlacement:
                            foreach (var item in AmbientHitableObjectType.lookup)
                            {
                                var newItem = new AmbientHitableObjectListItem(item.Value);

                                if (item.Value.ID == newId)
                                    selectedItem = newItem;

                                manageObjectList.AddElement(newItem);
                            }
                            break;
                        case PlacementType.LivingEntityPlacement:
                            foreach (var item in LivingEntityType.lookup)
                            {
                                var newItem = new LivingEntityListItem(item.Value);

                                if (item.Value.ID == newId)
                                    selectedItem = newItem;

                                manageObjectList.AddElement(newItem);
                            }
                            break;
                    }

                    manageObjectList.SelectElement(selectedItem);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                base.Update(gameTime);

                if (placementType != PlacementType.NoType && placementType != PlacementType.Inspect)
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
                else if (placementType == PlacementType.Inspect)
                {
                    inspectView.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);
                base.Draw(spriteBatch, gameTime);

                if(placementType != PlacementType.NoType && placementType != PlacementType.Inspect)
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
                else if (placementType == PlacementType.Inspect)
                {
                    inspectView.Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
