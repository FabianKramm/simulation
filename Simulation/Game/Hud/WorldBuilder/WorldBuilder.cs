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
    public class WorldBuilder: UIElement
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

        private TileSetSelectionView tileSetSelectionView;
        private ScrollableList tilesetSelectionList;

        private Texture2D backgroundOverlay;
        private Color backgroundColor = new Color(Color.Black, 0.2f);

        private Rect tilesetSelectionArea = new Rect(SimulationGame.Resolution.Width * 2 / 3, 0, SimulationGame.Resolution.Width / 3, SimulationGame.Resolution.Height);

        private PlacementType placementType = PlacementType.BlockPlacement;
        private PlacementMode placementMode = PlacementMode.ChooseTileset;

        public void LoadContent()
        {
            backgroundOverlay = new Texture2D(SimulationGame.Graphics.GraphicsDevice, 1, 1);
            backgroundOverlay.SetData(new Color[] { Color.White });

            tileSetSelectionView = new TileSetSelectionView(tilesetSelectionArea);
            tilesetSelectionList = new ScrollableList(tilesetSelectionArea);

            foreach(var tileset in tilesets)
            {
                var button = new Button(tileset, Point.Zero);

                button.ShowBorder = false;
                button.OnClick((Point position) =>
                {
                    placementMode = PlacementMode.CreateFromTileset;
                    tileSetSelectionView.SetTileSet(tileset, OnTileSetSelected);
                });

                tilesetSelectionList.AddElement(button);
            }

            OnKeyPress(Keys.Back, () =>
            {
                if(placementMode == PlacementMode.CreateFromTileset)
                {
                    placementMode = PlacementMode.ChooseTileset;
                }
            });
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

                switch(placementMode)
                {
                    case PlacementMode.ChooseTileset:
                        tilesetSelectionList.Update(gameTime);
                        break;
                    case PlacementMode.CreateFromTileset:
                        tileSetSelectionView.Update(gameTime);
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(SimulationGame.IsWorldBuilderOpen)
            {
                spriteBatch.Draw(backgroundOverlay, new Rectangle(0, 0, SimulationGame.Resolution.Width, SimulationGame.Resolution.Height), backgroundColor);

                switch (placementMode)
                {
                    case PlacementMode.ChooseTileset:
                        tilesetSelectionList.Draw(spriteBatch, gameTime);
                        break;
                    case PlacementMode.CreateFromTileset:
                        tileSetSelectionView.Draw(spriteBatch, gameTime);
                        break;
                }
            }
        }
    }
}
