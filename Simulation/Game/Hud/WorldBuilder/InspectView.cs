using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using Simulation.Util.UI;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Hud.WorldBuilder
{
    public class InspectView: UIElement
    {
        private Action<GameObject> gameObjectSelection;
        private Action<BlockType> blockSelection;

        public bool BlockSelectionAllowed = true;

        public Point SelectedBlockPosition = Point.Zero;
        public BlockType SelectedBlockType = null;

        public GameObject SelectedGameObject = null;

        public InspectView(Rect bounds)
        {
            Bounds = bounds;

            OnClick(handleOnClick);
            OnKeyPress(Keys.Escape, Deselect);
        }

        public void OnSelect(Action<GameObject> gameObjectSelection, Action<BlockType> blockSelection)
        {
            this.gameObjectSelection = gameObjectSelection;
            this.blockSelection = blockSelection;
        }

        private void handleOnClick(Point position)
        {
            SelectedBlockPosition = Point.Zero;
            SelectedBlockType = null;
            SelectedGameObject = null;

            SortedList<float, GameObject> sortedList = new SortedList<float, GameObject>();
            WorldPart worldPart = (SimulationGame.Player.InteriorID == Interior.Outside) ? SimulationGame.World.GetFromRealPoint((int)SimulationGame.RealWorldMousePosition.X, (int)SimulationGame.RealWorldMousePosition.Y) : (WorldPart)SimulationGame.World.InteriorManager.Get(SimulationGame.Player.InteriorID);

            foreach(var ambientObject in worldPart.AmbientObjects)
            {
                var ambientObjectType = AmbientObjectType.lookup[ambientObject.AmbientObjectType];
                var renderPosition = new Rect((int)(ambientObject.Position.X - ambientObjectType.SpriteOrigin.X), (int)(ambientObject.Position.Y - ambientObjectType.SpriteOrigin.Y), ambientObjectType.SpriteBounds.X, ambientObjectType.SpriteBounds.Y);

                if (renderPosition.Contains(SimulationGame.RealWorldMousePosition))
                {
                    sortedList.Add(ambientObjectType.HasDepth ? GeometryUtils.GetLayerDepthFromPosition(ambientObject.Position.X, ambientObject.Position.Y) : GeometryUtils.GetLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration), ambientObject);
                }
            }

            foreach (var hitableObject in worldPart.ContainedObjects)
            {
                Rect renderPosition = Rect.Empty;

                if (hitableObject is LivingEntity)
                {
                    var livingEntity = (LivingEntity)hitableObject;
                    var livingEntityType = LivingEntityType.lookup[livingEntity.LivingEntityType];

                    renderPosition = new Rect((int)(livingEntity.Position.X - livingEntityType.SpriteOrigin.X), (int)(livingEntity.Position.Y - livingEntityType.SpriteOrigin.Y), livingEntityType.SpriteBounds.X, livingEntityType.SpriteBounds.Y);
                }
                else if(hitableObject is AmbientHitableObject)
                {
                    var ambientHitableObject = (AmbientHitableObject)hitableObject;
                    var ambientHitableObjectType = AmbientHitableObjectType.lookup[ambientHitableObject.AmbientHitableObjectType];

                    renderPosition = new Rect((int)(ambientHitableObject.Position.X - ambientHitableObjectType.SpriteOrigin.X), (int)(ambientHitableObject.Position.Y - ambientHitableObjectType.SpriteOrigin.Y), ambientHitableObjectType.SpriteBounds.X, ambientHitableObjectType.SpriteBounds.Y);
                }

                if (renderPosition.Contains(SimulationGame.RealWorldMousePosition))
                {
                    sortedList.Add(GeometryUtils.GetLayerDepthFromPosition(hitableObject.Position.X, hitableObject.Position.Y), hitableObject);
                }
            }

            if(worldPart is WorldGridChunk)
            {
                var worldGridChunk = ((WorldGridChunk)worldPart);

                if(worldGridChunk.OverlappingObjects != null)
                    foreach (var hitableObject in worldGridChunk.OverlappingObjects)
                    {
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

                        if (renderPosition.Contains(SimulationGame.RealWorldMousePosition))
                        {
                            sortedList.Add(GeometryUtils.GetLayerDepthFromPosition(hitableObject.Position.X, hitableObject.Position.Y), hitableObject);
                        }
                    }
            }

            if(sortedList.Count == 0)
            {
                // We get the block
                SelectedBlockPosition = GeometryUtils.GetBlockFromReal((int)SimulationGame.RealWorldMousePosition.X, (int)SimulationGame.RealWorldMousePosition.Y);
                SelectedBlockType = BlockType.lookup[SimulationGame.World.GetBlockType(SelectedBlockPosition.X, SelectedBlockPosition.Y, SimulationGame.Player.InteriorID)];

                blockSelection?.Invoke(SelectedBlockType);
            }
            else
            {
                SelectedGameObject = sortedList.Values[sortedList.Count - 1];

                gameObjectSelection?.Invoke(SelectedGameObject);
            }
        }

        public void SelectGameObject(GameObject gameObject)
        {
            SelectedBlockPosition = Point.Zero;
            SelectedBlockType = null;
            SelectedGameObject = gameObject;
        }

        public void Deselect()
        {
            SelectedBlockPosition = Point.Zero;
            SelectedBlockType = null;
            SelectedGameObject = null;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SelectedGameObject != null)
            {
                if(SimulationGame.VisibleArea.Contains(SelectedGameObject.Position))
                {
                    var worldDrawPosition = Rectangle.Empty;

                    if(SelectedGameObject is AmbientObject)
                    {
                        var gameObject = (AmbientObject)SelectedGameObject;

                        worldDrawPosition = new Rectangle((int)(gameObject.Position.X - gameObject.GetObjectType().SpriteOrigin.X), (int)(gameObject.Position.Y - gameObject.GetObjectType().SpriteOrigin.Y), gameObject.GetObjectType().SpriteBounds.X, gameObject.GetObjectType().SpriteBounds.Y);
                    }
                    else if(SelectedGameObject is AmbientHitableObject)
                    {
                        var gameObject = (AmbientHitableObject)SelectedGameObject;

                        worldDrawPosition = new Rectangle((int)(gameObject.Position.X - gameObject.GetObjectType().SpriteOrigin.X), (int)(gameObject.Position.Y - gameObject.GetObjectType().SpriteOrigin.Y), gameObject.GetObjectType().SpriteBounds.X, gameObject.GetObjectType().SpriteBounds.Y);
                    }
                    else if(SelectedGameObject is LivingEntity)
                    {
                        var gameObject = (LivingEntity)SelectedGameObject;

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
        }
    }
}
