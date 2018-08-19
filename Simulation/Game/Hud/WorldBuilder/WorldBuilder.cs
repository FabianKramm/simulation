using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Game.Generator;
using Simulation.Game.Hud.WorldBuilder.ObjectListItems;
using Simulation.Game.MetaData;
using Simulation.Game.MetaData.World;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Game.Serialization.Objects;
using Simulation.Game.World;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using Simulation.Util.UI.Elements;
using System;
using System.Collections.Generic;
using System.IO;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class WorldBuilder: BaseUI
    {
        public static readonly Point BlockSize = new Point(16, 16);

        public enum PlacementType
        {
            NoType,
            Inspect,
            BlockPlacement,
            AmbientObjectPlacement,
            AmbientHitableObjectPlacement,
            LivingEntityPlacement,
            WorldPartDetails
        }

        public enum PlacementMode
        {
            NoPlacement,
            Manage,
            ChooseTileset,
            CreateFromTileset,
            CreateFromJson
        }

        private string[] tilesets;
        
        private Button blockTypeBtn;
        private Button ambientObjectTypeBtn;
        private Button ambientHitableObjectTypeBtn;
        private Button livingEntityTypeBtn;
        private Button worldPartBtn;

        private Button manageBtn;
        private Button createFromTilesetBtn;
        private Button createFromJsonBtn;

        // WorldPart
        private Button changePersistencyBtn;
        private Button createWorldLinkBtn;
        private Button createInteriorBtn;
        private Button changeInteriorDimensionsBtn;
        private Button removeInteriorBtn;

        // Inspect
        private Button editInstanceBtn;
        private Button removeInstanceBtn;
        private Button showInstanceTypeBtn;

        // Manage
        private Button editBtn;
        private Button removeBtn;
        private Button createNewFromBtn;

        // CreateFromTileset
        private Button createBtn;
        private Button createIfNotExistBtn;

        private BaseUI placeView;
        private InspectView inspectView;
        private TextView selectedObjectDetailTextView;
        private TextView worldPartDetailsTextView;

        private TileSetSelectionView tileSetSelectionView;
        private ScrollableList tilesetSelectionList;

        private ScrollableList manageObjectList;

        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);

        private Rect tilesetSelectionArea;

        private PlacementType placementType = PlacementType.Inspect;
        private PlacementMode placementMode = PlacementMode.NoPlacement;

        public void LoadContent()
        {
            tilesets = WorldBuilderUtils.GetTileSets(SimulationGame.ContentManager.RootDirectory + "\\Tiles");

            Bounds = new Rect(SimulationGame.Resolution.Width * 2 / 3, 0, SimulationGame.Resolution.Width / 3, SimulationGame.Resolution.Height);
            tilesetSelectionArea = new Rect(Bounds.X, Bounds.Y + 120, Bounds.Width - 10, Bounds.Height);

            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            placeView = new BaseUI()
            {
                Bounds = new Rect(0, 0, SimulationGame.Resolution.Width * 2 / 3, SimulationGame.Resolution.Height)
            };
            placeView.OnKeyPress(Keys.Escape, () =>
            {
                if(placementMode == PlacementMode.CreateFromTileset)
                {
                    tileSetSelectionView.Deselect();
                }
            });
            placeView.OnClick(placeObjectAtPosition);

            selectedObjectDetailTextView = new TextView(tilesetSelectionArea, "");
            worldPartDetailsTextView = new TextView(tilesetSelectionArea, "");

            inspectView = new InspectView(new Rect(0, 0, SimulationGame.Resolution.Width * 2 / 3, SimulationGame.Resolution.Height));
            inspectView.OnSelect(handleInspectGameObjectSelection, handleInspectBlockSelection, handleInspectWorldLinkSelection);

            tileSetSelectionView = new TileSetSelectionView(tilesetSelectionArea);
            tilesetSelectionList = new ScrollableList(tilesetSelectionArea);
            tilesetSelectionList.OnSelect((UIElement selectedElement) =>
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
            blockTypeBtn = new Button("Blocks", new Point(Bounds.X, Bounds.Y + 10));
            blockTypeBtn.OnClick(() => {
                placementType = PlacementType.BlockPlacement;
                handleManageBtnClick();
            });

            // Ambient Object Type Button
            ambientObjectTypeBtn = new Button("Ambient Objects", new Point(blockTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            ambientObjectTypeBtn.OnClick(() => {
                placementType = PlacementType.AmbientObjectPlacement;
                handleManageBtnClick();
            });

            // Ambient Hitable Object Type Button
            ambientHitableObjectTypeBtn = new Button("Hitable Objects", new Point(ambientObjectTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            ambientHitableObjectTypeBtn.OnClick(() => {
                placementType = PlacementType.AmbientHitableObjectPlacement;
                handleManageBtnClick();
            });

            // Ambient Hitable Object Type Button
            livingEntityTypeBtn = new Button("Living Entities", new Point(ambientHitableObjectTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            livingEntityTypeBtn.OnClick(() => {
                placementType = PlacementType.LivingEntityPlacement;
                handleManageBtnClick();
            });

            // World Part Details Button
            worldPartBtn = new Button("World Details", new Point(livingEntityTypeBtn.Bounds.Right + 10, Bounds.Y + 10));
            worldPartBtn.OnClick(() => {
                placementType = PlacementType.WorldPartDetails;
                placementMode = PlacementMode.NoPlacement;
            });

            // Manage Button
            manageBtn = new Button("Manage", new Point(Bounds.X, blockTypeBtn.Bounds.Bottom + 10));
            manageBtn.OnClick(handleManageBtnClick);

            // Create From Json
            createFromJsonBtn = new Button("Create From Json", new Point(manageBtn.Bounds.Right + 10, blockTypeBtn.Bounds.Bottom + 10));
            createFromJsonBtn.OnClick(createNewObjectFromJson);

            // Create From Tileset
            createFromTilesetBtn = new Button("Create From Tileset", new Point(createFromJsonBtn.Bounds.Right + 10, blockTypeBtn.Bounds.Bottom + 10));
            createFromTilesetBtn.OnClick(() => placementMode = PlacementMode.ChooseTileset);

            // Edit Btn
            editBtn = new Button("Edit", new Point(Bounds.X, manageBtn.Bounds.Bottom + 10));
            editBtn.OnClick(editObject);

            // Create New From Btn
            createNewFromBtn = new Button("Create New From", new Point(editBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            createNewFromBtn.OnClick(createNewFrom);

            // Remove Btn
            removeBtn = new Button("Remove", new Point(createNewFromBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            removeBtn.OnClick(removeObject);

            // Create If not exist Btn
            createIfNotExistBtn = new Button("Create If Not Exist", new Point(Bounds.X, manageBtn.Bounds.Bottom + 10));
            createIfNotExistBtn.OnClick(createNewObjectIfNotExists);

            // Create Btn
            createBtn = new Button("Create New", new Point(createIfNotExistBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            createBtn.OnClick(createNewObject);

            // Edit Instance Btn
            editInstanceBtn = new Button("Edit", new Point(Bounds.X, manageBtn.Bounds.Bottom + 10));
            editInstanceBtn.OnClick(handleEditInstanceBtnClick);

            // Remove Instance Btn
            removeInstanceBtn = new Button("Remove", new Point(editInstanceBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            removeInstanceBtn.OnClick(handleRemoveInstanceBtnClick);

            // Show Instance Type Btn
            showInstanceTypeBtn = new Button("Show Type", new Point(removeInstanceBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            showInstanceTypeBtn.OnClick(handleShowInstanceTypeBtnClick);

            // Change Persistency Btn
            changePersistencyBtn = new Button("Change Persistency", new Point(Bounds.X, worldPartBtn.Bounds.Bottom + 10));
            changePersistencyBtn.OnClick(handleChangePersistencyBtn);

            // Create WorldLink Btn
            createWorldLinkBtn = new Button("Create Worldlink", new Point(changePersistencyBtn.Bounds.Right + 10, worldPartBtn.Bounds.Bottom + 10));
            createWorldLinkBtn.OnClick(handleCreateWorldLinkBtn);

            // Create Interior Btn
            createInteriorBtn = new Button("Create Interior", new Point(createWorldLinkBtn.Bounds.Right + 10, worldPartBtn.Bounds.Bottom + 10));
            createInteriorBtn.OnClick(handleCreateInteriorBtn);

            // Change Interior Dimensions Btn
            changeInteriorDimensionsBtn = new Button("Change Dimensions", new Point(Bounds.X, manageBtn.Bounds.Bottom + 10));
            changeInteriorDimensionsBtn.OnClick(hanleChangeInteriorDimensionsBtn);

            // Delete Interior Btn
            removeInteriorBtn = new Button("Remove Interior", new Point(changeInteriorDimensionsBtn.Bounds.Right + 10, manageBtn.Bounds.Bottom + 10));
            removeInteriorBtn.OnClick(handleRemoveInteriorBtn);

            OnKeyPress(Keys.Delete, handleRemoveInstanceBtnClick);
            OnRightClick(() =>
            {
                if (placementMode == PlacementMode.CreateFromTileset)
                {
                    placementMode = PlacementMode.ChooseTileset;
                }
            });
            OnKeyPress(Keys.Back, () => {
                if(placementMode == PlacementMode.CreateFromTileset)
                {
                    placementMode = PlacementMode.ChooseTileset;
                }
            });

            AddElement(blockTypeBtn);
            AddElement(ambientObjectTypeBtn);
            AddElement(ambientHitableObjectTypeBtn);
            AddElement(livingEntityTypeBtn);
            AddElement(worldPartBtn);
        }

        private void hanleChangeInteriorDimensionsBtn()
        {
            var newInteriorDimensions = new Point(20, 20);
            var dialog = new InputDialog("New Interior Dimensions", JToken.FromObject(newInteriorDimensions, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newInteriorDimensions = SerializationUtils.Serializer.Deserialize<Point>(new JTokenReader(JToken.Parse(dialog.ResultText)));

                SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID).ChangeDimensions(newInteriorDimensions);
            }
        }

        private void handleRemoveInteriorBtn()
        {
            var confirmResult = System.Windows.Forms.MessageBox.Show("Are you sure to delete the interior?", "Confirm Delete!", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (confirmResult == System.Windows.Forms.DialogResult.Yes)
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID);

                if(interior.WorldLinks == null || interior.WorldLinks.Count == 0)
                {
                    SimulationGame.Player.UpdatePosition(new WorldPosition(0, 0, Interior.Outside));
                }

                // Check if durable entities are inside besides player
                if(interior.ContainedObjects != null) 
                    foreach(var containedObject in interior.ContainedObjects) 
                        if(containedObject is DurableEntity && containedObject is Player == false)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot delete interior because a durable entity is inside");
                            return;
                        }

                // Delete all WorldLinks
                if(interior.WorldLinks != null)
                {
                    bool playerTeleported = false;

                    WorldLink[] wordLinks = new WorldLink[interior.WorldLinks.Count];
                    interior.WorldLinks.Values.CopyTo(wordLinks, 0);

                    foreach (var worldLink in wordLinks)
                    {
                        if(playerTeleported == false)
                        {
                            SimulationGame.Player.UpdatePosition(worldLink.ToRealWorldPositionTo());
                            playerTeleported = true;
                        }

                        try
                        {
                            SimulationGame.World.UpdateWorldLink(worldLink);
                        }
                        catch(Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Couldn't delete WorldLink: " + e.Message);
                        }
                    }
                }

                // Unload and Erase File
                SimulationGame.World.InteriorManager.RemoveChunk(interior.ID);
                WorldLoader.EraseInterior(interior);
            }
        }

        private void handleChangePersistencyBtn()
        {
            var player = SimulationGame.Player;
            WorldPart currentWorldPart = player.InteriorID == Interior.Outside ? (WorldPart)SimulationGame.World.GetFromRealPoint((int)player.Position.X, (int)player.Position.Y) : SimulationGame.World.InteriorManager.Get(player.InteriorID);

            currentWorldPart.SetPersistent(!currentWorldPart.IsPersistent);

            if(currentWorldPart is WorldGridChunk)
            {
                var chunkPoint = GeometryUtils.GetChunkPosition((int)player.Position.X, (int)player.Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                WorldLoader.SaveWorldGridChunk(chunkPoint.X, chunkPoint.Y, (WorldGridChunk)currentWorldPart);
                WorldLoader.SaveWalkableGridChunk(chunkPoint.X, chunkPoint.Y, (WalkableGridChunk)SimulationGame.World.WalkableGrid.GetFromRealPoint(chunkPoint.X, chunkPoint.Y));
            }
            else
            {
                WorldLoader.SaveInterior(SimulationGame.World.InteriorManager.Get(player.InteriorID));
            }
        }

        private void handleCreateInteriorBtn()
        {
            var interiorDimensions = new Point(20, 20);
            var player = SimulationGame.Player;
            var currentWorldPart = player.InteriorID == Interior.Outside ? (WorldPart)SimulationGame.World.GetFromRealPoint((int)player.Position.X, (int)player.Position.Y) : SimulationGame.World.InteriorManager.Get(player.InteriorID);
            var dialog = new InputDialog("Interior Dimensions", JToken.FromObject(interiorDimensions, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                interiorDimensions = SerializationUtils.Serializer.Deserialize<Point>(new JTokenReader(JToken.Parse(dialog.ResultText)));
                
                var newInterior = new Interior(interiorDimensions);

                newInterior.SetPersistent(true);

                for (int i = 0; i < interiorDimensions.X; i++)
                    for (int j = 0; j < interiorDimensions.Y; j++)
                        newInterior.SetBlockType(i, j, 1);

                var interiorWorldLink = new WorldLink(new WorldPosition(0, 0, newInterior.ID), player.Position.ToBlockPosition());

                newInterior.AddWorldLink(interiorWorldLink);

                WorldLoader.SaveInterior(newInterior);

                currentWorldPart.SetPersistent(true);
                currentWorldPart.AddWorldLink(interiorWorldLink.SwapFromTo());
            }
        }

        private void handleCreateWorldLinkBtn()
        {
            var newWorldLink = new WorldLink()
            {
                FromBlock=SimulationGame.Player.BlockPosition,
                FromInteriorID=SimulationGame.Player.InteriorID,
                ToBlock=Point.Zero,
                ToInteriorID=Interior.Outside
            };

            var dialog = new InputDialog("Create WorldLink", JToken.FromObject(newWorldLink, SerializationUtils.Serializer).ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newWorldLink = SerializationUtils.Serializer.Deserialize<WorldLink>(new JTokenReader(JToken.Parse(dialog.ResultText)));
                SimulationGame.World.UpdateWorldLink(null, newWorldLink);
            }
        }

        private void handleInspectWorldLinkSelection(WorldLink worldLink)
        {
            placementType = PlacementType.Inspect;
            placementMode = PlacementMode.NoPlacement;
        }

        private void placeObjectAtPosition()
        {
            if(placementMode == PlacementMode.Manage)
            {
                WorldBuilderUtils.CreateObjectAtMousePosition(((ObjectListItem)manageObjectList.SelectedElement).GetObject());
            }
            else if(placementMode == PlacementMode.CreateFromTileset && tileSetSelectionView.SelectedObject != null)
            {
                WorldBuilderUtils.CreateObjectAtMousePosition(tileSetSelectionView.SelectedObject);
            }
        }

        private void handleInspectGameObjectSelection(List<GameObject> gameObject)
        {
            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                handleShowInstanceTypeBtnClick();

                placeView.OnClick(placeObjectAtPosition);
            }
            else
            {
                placementType = PlacementType.Inspect;
                placementMode = PlacementMode.NoPlacement;
            }
        }

        private void handleInspectBlockSelection(BlockType blockType)
        {
            placementType = PlacementType.BlockPlacement;
            placementMode = PlacementMode.Manage;
            manageObjectList.Clear();

            UIElement selectedItem = null;

            foreach (var item in BlockType.lookup)
            {
                var newItem = new BlockListItem(item.Value);

                if (item.Value == blockType)
                    selectedItem = newItem;

                manageObjectList.AddElement(newItem);
            }

            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
            {
                manageObjectList.SelectElement(selectedItem);

                placeView.OnClick(placeObjectAtPosition);
            }
            else
            {
                manageObjectList.ScrollToElement(selectedItem);
            }
        }
        
        private void handleShowInstanceTypeBtnClick()
        {
            var selectedObject = inspectView.SelectedGameObjects[0];
            
            placementMode = PlacementMode.Manage;
            manageObjectList.Clear();

            UIElement selectedItem = null;

            if(selectedObject is AmbientObject)
            {
                placementType = PlacementType.AmbientObjectPlacement;
                var selectedObjectType = ((AmbientObject)selectedObject).GetObjectType();

                foreach (var item in AmbientObjectType.lookup)
                {
                    var newItem = new AmbientObjectListItem(item.Value);

                    if (item.Value == selectedObjectType)
                        selectedItem = newItem;

                    manageObjectList.AddElement(newItem);
                }
            }
            else if(selectedObject is AmbientHitableObject)
            {
                placementType = PlacementType.AmbientHitableObjectPlacement;
                var selectedObjectType = ((AmbientHitableObject)selectedObject).GetObjectType();

                foreach (var item in AmbientHitableObjectType.lookup)
                {
                    var newItem = new AmbientHitableObjectListItem(item.Value);

                    if (item.Value == selectedObjectType)
                        selectedItem = newItem;

                    manageObjectList.AddElement(newItem);
                }
            }
            else if(selectedObject is LivingEntity)
            {
                placementType = PlacementType.LivingEntityPlacement;
                var selectedObjectType = ((LivingEntity)selectedObject).GetObjectType();

                foreach (var item in LivingEntityType.lookup)
                {
                    var newItem = new LivingEntityListItem(item.Value);

                    if (item.Value == selectedObjectType)
                        selectedItem = newItem;

                    manageObjectList.AddElement(newItem);
                }
            }

            manageObjectList.SelectElement(selectedItem);
        }

        private void handleRemoveInstanceBtnClick()
        {
            if(inspectView.SelectedGameObjects.Count > 0)
            {
                foreach(var selectedObject in inspectView.SelectedGameObjects)
                {
                    selectedObject.DisconnectFromWorld();
                    selectedObject.Destroy();
                }

                inspectView.Deselect();
            }
            else if(inspectView.SelectedWorldLink != null)
            {
                var SelectedWorldLink = inspectView.SelectedWorldLink;
                var confirmResult = System.Windows.Forms.MessageBox.Show("Are you sure to delete this object?", "Confirm Delete!", System.Windows.Forms.MessageBoxButtons.YesNo);

                if (confirmResult == System.Windows.Forms.DialogResult.Yes)
                {
                    inspectView.Deselect();
                    SimulationGame.World.UpdateWorldLink(SelectedWorldLink);
                }
            }
        }

        private void handleEditInstanceBtnClick()
        {
            if (inspectView.SelectedGameObjects.Count > 0)
            {
                var selectedObject = inspectView.SelectedGameObjects[0];
                var dialog = new InputDialog("Edit Object", WorldObjectSerializer.Serialize(selectedObject).ToString(Formatting.Indented));

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    inspectView.Deselect();
                    selectedObject.DisconnectFromWorld();
                    selectedObject.Destroy();

                    var newObject = WorldObjectSerializer.Deserialize(JObject.Parse(dialog.ResultText));

                    newObject.ConnectToWorld();
                    inspectView.SelectGameObject(newObject);
                }
            }
            else if (inspectView.SelectedWorldLink != null)
            {
                var SelectedWorldLink = inspectView.SelectedWorldLink;
                var dialog = new InputDialog("Edit Object", JToken.FromObject(SelectedWorldLink, SerializationUtils.Serializer).ToString(Formatting.Indented));

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    inspectView.Deselect();

                    var newWorldLink = SerializationUtils.Serializer.Deserialize<WorldLink>(new JTokenReader(JToken.Parse(dialog.ResultText)));
                    SimulationGame.World.UpdateWorldLink(SelectedWorldLink, newWorldLink);
                }
            }
        }

        private void handleManageBtnClick()
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

        private void removeObject()
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

        private void createNewFrom()
        {
            var selectedElement = manageObjectList.SelectedElement;
            object selectedObject = null;

            switch (placementType)
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

            int newId = WorldBuilderUtils.GenerateNewId(placementType);
            var selectedJToken = JObject.FromObject(selectedObject, SerializationUtils.Serializer);

            selectedJToken["ID"] = newId;

            var dialog = new InputDialog("Create New Object From", selectedJToken.ToString(Formatting.Indented));

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WorldBuilderUtils.ReplaceTypeFromString(placementType, dialog.ResultText);

                refreshListAndSelectId(newId);
            }
        }

        private void editObject()
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

                refreshListAndSelectId(((ObjectListItem)selectedElement).GetObject().ID);
            }
        }

        private void createNewObjectIfNotExists()
        {
            if (tileSetSelectionView.SelectedObject == null)
            {
                int newId = WorldBuilderUtils.CreateObject(placementType, placementMode, tileSetSelectionView);

                if(newId >= 0)
                {
                    switch (placementType)
                    {
                        case PlacementType.BlockPlacement:
                            tileSetSelectionView.SelectedObject = BlockType.lookup[newId];
                            break;
                        case PlacementType.AmbientObjectPlacement:
                            tileSetSelectionView.SelectedObject = AmbientObjectType.lookup[newId];
                            break;
                        case PlacementType.AmbientHitableObjectPlacement:
                            tileSetSelectionView.SelectedObject = AmbientHitableObjectType.lookup[newId];
                            break;
                    }
                }
            }
        }

        private void refreshListAndSelectId(int newId)
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

        private void createNewObjectFromJson()
        {
            int newId = WorldBuilderUtils.CreateObject(placementType, placementMode, tileSetSelectionView, true);

            if (newId != -1)
            {
                refreshListAndSelectId(newId);
            }
        }

        private void createNewObject()
        {
            int newId = WorldBuilderUtils.CreateObject(placementType, placementMode, tileSetSelectionView);

            if (newId != -1 && placementMode == PlacementMode.CreateFromTileset)
            {
                refreshListAndSelectId(newId);
            }
        }

        private void setSelectedButtonColor()
        {
            blockTypeBtn.TextColor = Color.White;
            ambientObjectTypeBtn.TextColor = Color.White;
            ambientHitableObjectTypeBtn.TextColor = Color.White;
            livingEntityTypeBtn.TextColor = Color.White;
            worldPartBtn.TextColor = Color.White;

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    blockTypeBtn.TextColor = Color.IndianRed;
                    break;
                case PlacementType.AmbientObjectPlacement:
                    ambientObjectTypeBtn.TextColor = Color.IndianRed;
                    break;
                case PlacementType.AmbientHitableObjectPlacement:
                    ambientHitableObjectTypeBtn.TextColor = Color.IndianRed;
                    break;
                case PlacementType.LivingEntityPlacement:
                    livingEntityTypeBtn.TextColor = Color.IndianRed;
                    break;
                case PlacementType.WorldPartDetails:
                    worldPartBtn.TextColor = Color.IndianRed;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                base.Update(gameTime);

                setSelectedButtonColor();

                if (placementType != PlacementType.NoType && placementType != PlacementType.Inspect && placementType != PlacementType.WorldPartDetails)
                {
                    switch (placementMode)
                    {
                        case PlacementMode.Manage:
                            manageObjectList.Update(gameTime);

                            if (manageObjectList.SelectedElement != null)
                            {
                                editBtn.Update(gameTime);
                                createNewFromBtn.Update(gameTime);
                                removeBtn.Update(gameTime);
                            }
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Update(gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Update(gameTime);

                            if (tileSetSelectionView.SelectedSpritePosition != null)
                            {
                                createBtn.Update(gameTime);
                                createIfNotExistBtn.Update(gameTime);
                            }
                            
                            break;
                    }

                    manageBtn.Update(gameTime);
                    createFromJsonBtn.Update(gameTime);

                    if (placementType != PlacementType.LivingEntityPlacement)
                        createFromTilesetBtn.Update(gameTime);
                }
                else if (placementType == PlacementType.WorldPartDetails)
                {
                    changePersistencyBtn.Update(gameTime);
                    createWorldLinkBtn.Update(gameTime);
                    createInteriorBtn.Update(gameTime);

                    if (SimulationGame.Player.InteriorID != Interior.Outside)
                    {
                        changeInteriorDimensionsBtn.Update(gameTime);
                        removeInteriorBtn.Update(gameTime);
                    }

                    var player = SimulationGame.Player;
                    WorldPart currentWorldPart = player.InteriorID == Interior.Outside ? (WorldPart)SimulationGame.World.GetFromRealPoint((int)player.Position.X, (int)player.Position.Y) : SimulationGame.World.InteriorManager.Get(player.InteriorID);

                    worldPartDetailsTextView.SetText(
                        "CurrentBlock: " + player.BlockPosition.X + "," + player.BlockPosition.Y + "\n" +
                        "IsPersistent: " + currentWorldPart.IsPersistent + "\n" + 
                        "InteriorID: " + player.InteriorID + "\n" +
                        (player.InteriorID != Interior.Outside ? ("Dimensions: " + ((Interior)currentWorldPart).Dimensions + "\n") : "")
                    );
                    worldPartDetailsTextView.Update(gameTime);
                }
                else if (placementType == PlacementType.Inspect)
                {
                    if (inspectView.SelectedGameObjects.Count > 0)
                    {
                        selectedObjectDetailTextView.SetText(WorldObjectSerializer.Serialize(inspectView.SelectedGameObjects[0]).ToString(Formatting.Indented));

                        editInstanceBtn.Update(gameTime);
                        removeInstanceBtn.Update(gameTime);
                        showInstanceTypeBtn.Update(gameTime);
                    }
                    else if(inspectView.SelectedWorldLink != null)
                    {
                        selectedObjectDetailTextView.SetText(JToken.FromObject(inspectView.SelectedWorldLink, SerializationUtils.Serializer).ToString(Formatting.Indented));

                        editInstanceBtn.Update(gameTime);
                        removeInstanceBtn.Update(gameTime);
                    }

                    selectedObjectDetailTextView.Update(gameTime);
                }

                if ((placementMode == PlacementMode.Manage && manageObjectList.SelectedElement != null) || 
                    (placementMode == PlacementMode.CreateFromTileset && tileSetSelectionView.SelectedObject != null))
                {
                    placeView.Update(gameTime);
                }
                else
                {
                    inspectView.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                spriteBatch.Draw(backgroundOverlay, Bounds.ToXnaRectangle(), backgroundColor);
                base.Draw(spriteBatch, gameTime);

                if(placementType != PlacementType.NoType && placementType != PlacementType.Inspect && placementType != PlacementType.WorldPartDetails)
                {
                    switch (placementMode)
                    {
                        case PlacementMode.Manage:
                            manageObjectList.Draw(spriteBatch, gameTime);

                            if (manageObjectList.SelectedElement != null)
                            {
                                editBtn.Draw(spriteBatch, gameTime);
                                createNewFromBtn.Draw(spriteBatch, gameTime);
                                removeBtn.Draw(spriteBatch, gameTime);
                            }   
                            break;
                        case PlacementMode.ChooseTileset:
                            tilesetSelectionList.Draw(spriteBatch, gameTime);
                            break;
                        case PlacementMode.CreateFromTileset:
                            tileSetSelectionView.Draw(spriteBatch, gameTime);

                            if (tileSetSelectionView.SelectedSpritePosition != null)
                            {
                                createBtn.Draw(spriteBatch, gameTime);
                                createIfNotExistBtn.Draw(spriteBatch, gameTime);
                            }
                                
                            break;
                    }

                    manageBtn.Draw(spriteBatch, gameTime);
                    createFromJsonBtn.Draw(spriteBatch, gameTime);

                    if(placementType != PlacementType.LivingEntityPlacement)
                        createFromTilesetBtn.Draw(spriteBatch, gameTime);
                }
                else if(placementType == PlacementType.WorldPartDetails)
                {
                    changePersistencyBtn.Draw(spriteBatch, gameTime);
                    createWorldLinkBtn.Draw(spriteBatch, gameTime);
                    createInteriorBtn.Draw(spriteBatch, gameTime);

                    if(SimulationGame.Player.InteriorID != Interior.Outside)
                    {
                        changeInteriorDimensionsBtn.Draw(spriteBatch, gameTime);
                        removeInteriorBtn.Draw(spriteBatch, gameTime);
                    }

                    worldPartDetailsTextView.Draw(spriteBatch, gameTime);
                }
                else if (placementType == PlacementType.Inspect)
                {
                    if (inspectView.SelectedGameObjects.Count > 0)
                    {
                        editInstanceBtn.Draw(spriteBatch, gameTime);
                        removeInstanceBtn.Draw(spriteBatch, gameTime);
                        showInstanceTypeBtn.Draw(spriteBatch, gameTime);
                        selectedObjectDetailTextView.Draw(spriteBatch, gameTime);
                    }
                    else if (inspectView.SelectedWorldLink != null)
                    {
                        editInstanceBtn.Draw(spriteBatch, gameTime);
                        removeInstanceBtn.Draw(spriteBatch, gameTime);

                        selectedObjectDetailTextView.Draw(spriteBatch, gameTime);
                    }
                }

                if((placementMode == PlacementMode.Manage && manageObjectList.SelectedElement != null) ||
                    (placementMode == PlacementMode.CreateFromTileset && tileSetSelectionView.SelectedObject != null))
                {
                    placeView.Draw(spriteBatch, gameTime);

                    if (placeView.Bounds.Contains(SimulationGame.MouseState.Position))
                    {
                        if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                        {
                            WorldBuilderUtils.DrawPreview(spriteBatch, SimulationGame.MouseState.Position.ToVector2(), (placementMode == PlacementMode.Manage) ? ((ObjectListItem)manageObjectList.SelectedElement).GetObject() : tileSetSelectionView.SelectedObject);
                        }
                        else
                        {
                            var isBlockSelected = false;

                            if(placementMode == PlacementMode.Manage)
                            {
                                if(((ObjectListItem)manageObjectList.SelectedElement) is BlockListItem)
                                    isBlockSelected = true;
                            }
                            else if(placementMode == PlacementMode.CreateFromTileset)
                            {
                                if (tileSetSelectionView.SelectedObject is BlockType)
                                    isBlockSelected = true;
                            }

                            if(isBlockSelected)
                            {
                                var worldBlockPosition = GeometryUtils.GetBlockFromReal((int)SimulationGame.RealWorldMousePosition.X, (int)SimulationGame.RealWorldMousePosition.Y);

                                WorldBuilderUtils.DrawPreview(spriteBatch, SimulationGame.ConvertWorldPositionToUIPosition(worldBlockPosition.X * WorldGrid.BlockSize.X, worldBlockPosition.Y * WorldGrid.BlockSize.Y), (placementMode == PlacementMode.Manage) ? ((ObjectListItem)manageObjectList.SelectedElement).GetObject() : tileSetSelectionView.SelectedObject);
                            }
                            else
                            {
                                var worldBlockPosition = GeometryUtils.GetChunkPosition((int)SimulationGame.RealWorldMousePosition.X, (int)SimulationGame.RealWorldMousePosition.Y, 16, 16);

                                WorldBuilderUtils.DrawPreview(spriteBatch, SimulationGame.ConvertWorldPositionToUIPosition(worldBlockPosition.X * 16, worldBlockPosition.Y * 16), (placementMode == PlacementMode.Manage) ? ((ObjectListItem)manageObjectList.SelectedElement).GetObject() : tileSetSelectionView.SelectedObject);
                            }
                        }
                    }
                }
                else
                {
                    inspectView.Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
