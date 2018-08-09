using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.MetaData;
using Simulation.Game.MetaData.World;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class InspectView: UIElement
    {
        private Action<List<GameObject>> gameObjectSelection;
        private Action<BlockType> blockSelection;
        private Action<WorldLink> worldLinkSelection;

        public bool BlockSelectionAllowed = true;

        public Point SelectedBlockPosition = Point.Zero;
        public BlockType SelectedBlockType = null;
        public WorldLink SelectedWorldLink = null;
        public List<GameObject> SelectedGameObjects = new List<GameObject>();

        private Point? startDragPosition = null;

        public InspectView(Rect bounds)
        {
            Bounds = bounds;

            OnMouseMove(handleOnMouseMove);
            OnKeyPress(Keys.Escape, Deselect);

            OnKeyHold(Keys.Left, handleKeyLeft, TimeSpan.FromMilliseconds(200));
            OnKeyHold(Keys.Right, handleKeyRight, TimeSpan.FromMilliseconds(200));
            OnKeyHold(Keys.Up, handleKeyUp, TimeSpan.FromMilliseconds(200));
            OnKeyHold(Keys.Down, handleKeyDown, TimeSpan.FromMilliseconds(200));
        }

        private void handleKeyLeft()
        {
            if(SelectedGameObjects.Count > 0)
            {
                foreach(var gameObject in SelectedGameObjects)
                {
                    gameObject.DisconnectFromWorld();

                    WorldPosition newPosition;

                    if(SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        newPosition = new WorldPosition(gameObject.Position.X - 1, gameObject.Position.Y, gameObject.InteriorID);
                    }
                    else
                    {
                        newPosition = new WorldPosition(gameObject.BlockPosition.X - 1, gameObject.BlockPosition.Y, gameObject.InteriorID).ToRealPosition();
                    }

                    gameObject.UpdatePosition(newPosition);
                    gameObject.ConnectToWorld();
                }
            }

            if(SelectedWorldLink != null)
            {
                WorldLink clonedSelected = SelectedWorldLink.Clone();

                clonedSelected.FromBlock.X -= 1;

                SimulationGame.World.UpdateWorldLink(SelectedWorldLink, clonedSelected);
                SelectedWorldLink = clonedSelected;
            }
        }

        private void handleKeyRight()
        {
            if (SelectedGameObjects.Count > 0)
            {
                foreach (var gameObject in SelectedGameObjects)
                {
                    gameObject.DisconnectFromWorld();

                    WorldPosition newPosition;

                    if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        newPosition = new WorldPosition(gameObject.Position.X + 1, gameObject.Position.Y, gameObject.InteriorID);
                    }
                    else
                    {
                        newPosition = new WorldPosition(gameObject.BlockPosition.X + 1, gameObject.BlockPosition.Y, gameObject.InteriorID).ToRealPosition();
                    }

                    gameObject.UpdatePosition(newPosition);
                    gameObject.ConnectToWorld();
                }
            }

            if (SelectedWorldLink != null)
            {
                WorldLink clonedSelected = SelectedWorldLink.Clone();

                clonedSelected.FromBlock.X += 1;

                SimulationGame.World.UpdateWorldLink(SelectedWorldLink, clonedSelected);
                SelectedWorldLink = clonedSelected;
            }
        }

        private void handleKeyUp()
        {
            if (SelectedGameObjects.Count > 0)
            {
                foreach (var gameObject in SelectedGameObjects)
                {
                    gameObject.DisconnectFromWorld();

                    WorldPosition newPosition;

                    if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        newPosition = new WorldPosition(gameObject.Position.X, gameObject.Position.Y - 1, gameObject.InteriorID);
                    }
                    else
                    {
                        newPosition = new WorldPosition(gameObject.BlockPosition.X, gameObject.BlockPosition.Y - 1, gameObject.InteriorID).ToRealPosition();
                    }

                    gameObject.UpdatePosition(newPosition);
                    gameObject.ConnectToWorld();
                }
            }

            if (SelectedWorldLink != null)
            {
                WorldLink clonedSelected = SelectedWorldLink.Clone();

                clonedSelected.FromBlock.Y -= 1;

                SimulationGame.World.UpdateWorldLink(SelectedWorldLink, clonedSelected);
                SelectedWorldLink = clonedSelected;
            }
        }

        private void handleKeyDown()
        {
            if (SelectedGameObjects.Count > 0)
            {
                foreach (var gameObject in SelectedGameObjects)
                {
                    gameObject.DisconnectFromWorld();

                    WorldPosition newPosition;

                    if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                    {
                        newPosition = new WorldPosition(gameObject.Position.X, gameObject.Position.Y + 1, gameObject.InteriorID);
                    }
                    else
                    {
                        newPosition = new WorldPosition(gameObject.BlockPosition.X, gameObject.BlockPosition.Y + 1, gameObject.InteriorID).ToRealPosition();
                    }

                    gameObject.UpdatePosition(newPosition);
                    gameObject.ConnectToWorld();
                }
            }

            if (SelectedWorldLink != null)
            {
                WorldLink clonedSelected = SelectedWorldLink.Clone();

                clonedSelected.FromBlock.Y += 1;

                SimulationGame.World.UpdateWorldLink(SelectedWorldLink, clonedSelected);
                SelectedWorldLink = clonedSelected;
            }
        }

        public void OnSelect(Action<List<GameObject>> gameObjectSelection, Action<BlockType> blockSelection, Action<WorldLink> worldLinkSelection)
        {
            this.gameObjectSelection = gameObjectSelection;
            this.blockSelection = blockSelection;
            this.worldLinkSelection = worldLinkSelection;
        }

        private void handleOnMouseMove(MouseMoveEvent mouseMoveEvent)
        {
            if(mouseMoveEvent.LeftButtonDown && startDragPosition == null)
            {
                startDragPosition = SimulationGame.RealWorldMousePosition.ToPoint();
            }

            if(mouseMoveEvent.LeftButtonDown == false && startDragPosition != null)
            {
                selectGameObjects(ShapeCollision.ConvertLineToRect(startDragPosition ?? Point.Zero, SimulationGame.RealWorldMousePosition.ToPoint()));
                startDragPosition = null;
            }
        }

        private void addSelectedObjectsFromWorldPart(Rect selectionRect, WorldPart worldPart, bool deselect = false)
        {
            if (worldPart.AmbientObjects != null)
                foreach (var ambientObject in worldPart.AmbientObjects)
                {
                    var ambientObjectType = AmbientObjectType.lookup[ambientObject.AmbientObjectType];
                    var renderPosition = new Rect((int)(ambientObject.Position.X - ambientObjectType.SpriteOrigin.X), (int)(ambientObject.Position.Y - ambientObjectType.SpriteOrigin.Y), ambientObjectType.SpriteBounds.X, ambientObjectType.SpriteBounds.Y);

                    if (renderPosition.Intersects(selectionRect))
                    {
                        if(deselect)
                        {
                            SelectedGameObjects.Remove(ambientObject);
                        }
                        else
                        {
                            SelectedGameObjects.Add(ambientObject);
                        }
                    }
                }

            if (worldPart.ContainedObjects != null)
                foreach (var hitableObject in worldPart.ContainedObjects)
                {
                    if (hitableObject is Player)
                        continue;

                    Rect renderPosition = Rect.Empty;

                    if (hitableObject is LivingEntity)
                    {
                        var livingEntity = (LivingEntity)hitableObject;
                        var livingEntityType = LivingEntityType.lookup[livingEntity.LivingEntityType];

                        renderPosition = new Rect((int)(livingEntity.Position.X - livingEntityType.SpriteOrigin.X), (int)(livingEntity.Position.Y - livingEntityType.SpriteOrigin.Y), livingEntityType.SpriteBounds.X, livingEntityType.SpriteBounds.Y);
                    }
                    else if (hitableObject is AmbientHitableObject)
                    {
                        var ambientHitableObject = (AmbientHitableObject)hitableObject;
                        var ambientHitableObjectType = AmbientHitableObjectType.lookup[ambientHitableObject.AmbientHitableObjectType];

                        renderPosition = new Rect((int)(ambientHitableObject.Position.X - ambientHitableObjectType.SpriteOrigin.X), (int)(ambientHitableObject.Position.Y - ambientHitableObjectType.SpriteOrigin.Y), ambientHitableObjectType.SpriteBounds.X, ambientHitableObjectType.SpriteBounds.Y);
                    }

                    if (renderPosition.Intersects(selectionRect))
                    {
                        if (deselect)
                        {
                            SelectedGameObjects.Remove(hitableObject);
                        }
                        else
                        {
                            SelectedGameObjects.Add(hitableObject);
                        }
                    }
                }

            if (worldPart is WorldGridChunk)
            {
                var worldGridChunk = ((WorldGridChunk)worldPart);

                if (worldGridChunk.OverlappingObjects != null)
                    foreach (var hitableObject in worldGridChunk.OverlappingObjects)
                    {
                        if (hitableObject is Player)
                            continue;

                        Rect renderPosition = Rect.Empty;

                        if (hitableObject is LivingEntity)
                        {
                            var livingEntity = (LivingEntity)hitableObject;
                            var livingEntityType = LivingEntityType.lookup[livingEntity.LivingEntityType];

                            renderPosition = new Rect((int)(livingEntity.Position.X - livingEntityType.SpriteOrigin.X), (int)(livingEntity.Position.Y - livingEntityType.SpriteOrigin.Y), livingEntityType.SpriteBounds.X, livingEntityType.SpriteBounds.Y);
                        }
                        else if (hitableObject is AmbientHitableObject)
                        {
                            var ambientHitableObject = (AmbientHitableObject)hitableObject;
                            var ambientHitableObjectType = AmbientHitableObjectType.lookup[ambientHitableObject.AmbientHitableObjectType];

                            renderPosition = new Rect((int)(ambientHitableObject.Position.X - ambientHitableObjectType.SpriteOrigin.X), (int)(ambientHitableObject.Position.Y - ambientHitableObjectType.SpriteOrigin.Y), ambientHitableObjectType.SpriteBounds.X, ambientHitableObjectType.SpriteBounds.Y);
                        }

                        if (renderPosition.Intersects(selectionRect)) 
                        {
                            if (deselect)
                            {
                                SelectedGameObjects.Remove(hitableObject);
                            }
                            else if(SelectedGameObjects.Contains(hitableObject) == false)
                            {
                                SelectedGameObjects.Add(hitableObject);
                            }
                        }
                    }
            }
        }

        private void selectGameObjects(Rect selectionRect)
        {
            // Is Deselection Mode?
            if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftShift) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightShift))
            {
                if (SimulationGame.Player.InteriorID == Interior.Outside)
                {
                    // Check collision with interactive && contained objects
                    Point chunkTopLeft = GeometryUtils.GetChunkPosition(selectionRect.Left, selectionRect.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    Point chunkBottomRight = GeometryUtils.GetChunkPosition(selectionRect.Right, selectionRect.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                    for (int chunkX = chunkTopLeft.X - 1; chunkX <= chunkBottomRight.X + 1; chunkX++)
                        for (int chunkY = chunkTopLeft.Y - 1; chunkY <= chunkBottomRight.Y + 1; chunkY++)
                        {
                            WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                            addSelectedObjectsFromWorldPart(selectionRect, worldGridChunk, true);
                        }
                }
                else
                {
                    addSelectedObjectsFromWorldPart(selectionRect, SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID), true);
                }

                if (SelectedGameObjects.Count > 0)
                    gameObjectSelection?.Invoke(SelectedGameObjects);
            }
            else
            {
                SelectedBlockPosition = Point.Zero;
                SelectedBlockType = null;
                SelectedWorldLink = null;
                SelectedGameObjects.Clear();

                if (SimulationGame.Player.InteriorID == Interior.Outside)
                {
                    // Check collision with interactive && contained objects
                    Point chunkTopLeft = GeometryUtils.GetChunkPosition(selectionRect.Left, selectionRect.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    Point chunkBottomRight = GeometryUtils.GetChunkPosition(selectionRect.Right, selectionRect.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                    for (int chunkX = chunkTopLeft.X - 1; chunkX <= chunkBottomRight.X + 1; chunkX++)
                        for (int chunkY = chunkTopLeft.Y - 1; chunkY <= chunkBottomRight.Y + 1; chunkY++)
                        {
                            WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                            addSelectedObjectsFromWorldPart(selectionRect, worldGridChunk);
                        }
                }
                else
                {
                    addSelectedObjectsFromWorldPart(selectionRect, SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID));
                }

                if (SelectedGameObjects.Count == 0)
                {
                    WorldPart worldPart = (SimulationGame.Player.InteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint((int)selectionRect.X, (int)selectionRect.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID);

                    if (worldPart.WorldLinks != null)
                        foreach (var worldLinkItem in worldPart.WorldLinks)
                        {
                            Rect renderPosition = new Rect(worldLinkItem.Value.FromBlock.X * WorldGrid.BlockSize.X, worldLinkItem.Value.FromBlock.Y * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                            if (renderPosition.Contains(selectionRect.GetPosition()))
                            {
                                SelectedWorldLink = worldLinkItem.Value;
                                worldLinkSelection?.Invoke(SelectedWorldLink);

                                return;
                            }
                        }

                    // We get the block
                    SelectedBlockPosition = GeometryUtils.GetBlockFromReal((int)SimulationGame.RealWorldMousePosition.X, (int)SimulationGame.RealWorldMousePosition.Y);
                    SelectedBlockType = BlockType.lookup[SimulationGame.World.GetBlockType(SelectedBlockPosition.X, SelectedBlockPosition.Y, SimulationGame.Player.InteriorID)];

                    blockSelection?.Invoke(SelectedBlockType);
                }
                else
                {
                    gameObjectSelection?.Invoke(SelectedGameObjects);
                }
            }
        }

        public void SelectGameObject(GameObject gameObject)
        {
            SelectedBlockPosition = Point.Zero;
            SelectedBlockType = null;
            SelectedGameObjects.Add(gameObject);
            SelectedWorldLink = null;
        }

        public void Deselect()
        {
            SelectedBlockPosition = Point.Zero;
            SelectedBlockType = null;
            SelectedGameObjects.Clear();
            SelectedWorldLink = null;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SelectedGameObjects.Count > 0)
            {
                foreach(var selectedGameObject in SelectedGameObjects)
                    if(SimulationGame.VisibleArea.Contains(selectedGameObject.Position))
                    {
                        var worldDrawPosition = Rectangle.Empty;

                        if(selectedGameObject is AmbientObject)
                        {
                            var gameObject = (AmbientObject)selectedGameObject;

                            worldDrawPosition = new Rectangle((int)(gameObject.Position.X - gameObject.GetObjectType().SpriteOrigin.X), (int)(gameObject.Position.Y - gameObject.GetObjectType().SpriteOrigin.Y), gameObject.GetObjectType().SpriteBounds.X, gameObject.GetObjectType().SpriteBounds.Y);
                        }
                        else if(selectedGameObject is AmbientHitableObject)
                        {
                            var gameObject = (AmbientHitableObject)selectedGameObject;

                            worldDrawPosition = new Rectangle((int)(gameObject.Position.X - gameObject.GetObjectType().SpriteOrigin.X), (int)(gameObject.Position.Y - gameObject.GetObjectType().SpriteOrigin.Y), gameObject.GetObjectType().SpriteBounds.X, gameObject.GetObjectType().SpriteBounds.Y);
                        }
                        else if(selectedGameObject is LivingEntity)
                        {
                            var gameObject = (LivingEntity)selectedGameObject;

                            worldDrawPosition = new Rectangle((int)(gameObject.Position.X - gameObject.GetObjectType().SpriteOrigin.X), (int)(gameObject.Position.Y - gameObject.GetObjectType().SpriteOrigin.Y), gameObject.GetObjectType().SpriteBounds.X, gameObject.GetObjectType().SpriteBounds.Y);
                        }

                        var uiPosition = SimulationGame.ConvertWorldPositionToUIPosition(worldDrawPosition.X, worldDrawPosition.Y);
                        SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle((int)uiPosition.X, (int)uiPosition.Y, worldDrawPosition.Width, worldDrawPosition.Height), Color.Yellow);
                    }
            }
            else if(SelectedBlockType != null)
            {
                var uiPosition = SimulationGame.ConvertWorldPositionToUIPosition(SelectedBlockPosition.X * WorldGrid.BlockSize.X, SelectedBlockPosition.Y * WorldGrid.BlockSize.Y);

                SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle((int)uiPosition.X, (int)uiPosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y), Color.Blue);
            }
            else if(SelectedWorldLink != null)
            {
                var uiPosition = SimulationGame.ConvertWorldPositionToUIPosition(SelectedWorldLink.FromBlock.X * WorldGrid.BlockSize.X, SelectedWorldLink.FromBlock.Y * WorldGrid.BlockSize.Y);

                SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle((int)uiPosition.X, (int)uiPosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y), Color.Orange);
            }

            if(startDragPosition != null)
            {
                var _startDragPosition = startDragPosition ?? Point.Zero;
                var uiStartDragPosition = SimulationGame.ConvertWorldPositionToUIPosition(_startDragPosition.X, _startDragPosition.Y).ToPoint();
                var selectionRect = ShapeCollision.ConvertLineToRect(uiStartDragPosition, SimulationGame.MouseState.Position);

                if (selectionRect.Width > 20 || selectionRect.Height > 20)
                    SimulationGame.PrimitiveDrawer.Rectangle(selectionRect.ToXnaRectangle(), Color.White);
            }
        }
    }
}
