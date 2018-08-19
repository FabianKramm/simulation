using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.MetaData.World;
using Simulation.Game.Objects;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util.Dialog;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using static Simulation.Game.Hud.WorldBuilder.WorldBuilder;

namespace Simulation.Game.Hud.WorldBuilder
{
    public static class WorldBuilderUtils
    {
        public static string[] GetTileSets(string path)
        {
            List<string> tilesets = new List<string>();

            var files = Directory.GetFiles(path);

            foreach (var file in files)
                tilesets.Add(file.Substring(SimulationGame.ContentManager.RootDirectory.Length + 1, file.Length - SimulationGame.ContentManager.RootDirectory.Length - 5));

            var directories = Directory.GetDirectories(path);

            foreach (var directory in directories)
                tilesets.AddRange(GetTileSets(directory));

            return tilesets.ToArray();
        }

        public static void CreateObjectAtMousePosition(MetaDataType metaDataType)
        {
            var realPosition = SimulationGame.RealWorldMousePosition;

            if (metaDataType is BlockType)
            {
                var blockPosition = GeometryUtils.GetBlockFromReal((int)realPosition.X, (int)realPosition.Y);
                var blockType = (BlockType)metaDataType;

                SimulationGame.World.SetBlockType(blockPosition.X, blockPosition.Y, SimulationGame.Player.InteriorID, blockType.ID);
            }
            else
            {
                if (!SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) && !SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                {
                    var blockPosition = GeometryUtils.GetChunkPosition((int)realPosition.X, (int)realPosition.Y, 16, 16);

                    realPosition = new Vector2(blockPosition.X * 16, blockPosition.Y * 16);
                }

                var newWorldPosition = new WorldPosition(realPosition.X, realPosition.Y, SimulationGame.Player.InteriorID);
                GameObject gameObject = null;

                if (metaDataType is AmbientObjectType)
                {
                    gameObject = AmbientObjectType.Create(newWorldPosition, (AmbientObjectType)metaDataType);
                }
                else if (metaDataType is AmbientHitableObjectType)
                {
                    gameObject = AmbientHitableObjectType.Create(newWorldPosition, (AmbientHitableObjectType)metaDataType);
                }
                else if (metaDataType is LivingEntityType)
                {
                    gameObject = LivingEntityType.Create(newWorldPosition, (LivingEntityType)metaDataType);
                }

                gameObject.ConnectToWorld();
            }
        } 

        public static int GenerateNewId(WorldBuilder.PlacementType placementType)
        {
            int highestNumber = -1;

            switch (placementType)
            {
                case WorldBuilder.PlacementType.BlockPlacement:
                    foreach (var blockTypeItem in BlockType.lookup)
                        if (blockTypeItem.Value.ID > highestNumber)
                            highestNumber = blockTypeItem.Value.ID;
                    break;
                case WorldBuilder.PlacementType.AmbientObjectPlacement:
                    foreach (var ambientObjectType in AmbientObjectType.lookup)
                        if (ambientObjectType.Value.ID > highestNumber)
                            highestNumber = ambientObjectType.Value.ID;
                    break;
                case WorldBuilder.PlacementType.AmbientHitableObjectPlacement:
                    foreach (var ambientHitableObjectType in AmbientHitableObjectType.lookup)
                        if (ambientHitableObjectType.Value.ID > highestNumber)
                            highestNumber = ambientHitableObjectType.Value.ID;
                    break;
                case WorldBuilder.PlacementType.LivingEntityPlacement:
                    foreach (var livingEntityType in LivingEntityType.lookup)
                        if (livingEntityType.Value.ID > highestNumber)
                            highestNumber = livingEntityType.Value.ID;
                    break;
            }

            return highestNumber + 1;
        }

        public static int CreateObject(PlacementType placementType, WorldBuilder.PlacementMode placementMode, TileSetSelectionView tileSetSelectionView, bool showDialog = false)
        {
            string spritePath = null;
            Point spritePosition = Point.Zero;
            Point spriteBounds = Point.Zero;

            if (placementMode == WorldBuilder.PlacementMode.CreateFromTileset)
            {
                spritePath = tileSetSelectionView.SelectedSpritePath;
                spritePosition = tileSetSelectionView.SelectedSpritePosition ?? Point.Zero;
                spriteBounds = tileSetSelectionView.SelectedSpriteBounds;
            }

            object selectedObject = null;
            int newId = GenerateNewId(placementType);

            switch (placementType)
            {
                case PlacementType.BlockPlacement:
                    if(spriteBounds.X != WorldGrid.BlockSize.X || spriteBounds.Y != WorldGrid.BlockSize.Y)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot create block with blocksize != 32,32");

                        return -1;
                    }

                    selectedObject = new BlockType()
                    {
                        ID = newId,
                        Name = "Block" + newId,
                        SpritePath = spritePath,
                        SpritePosition = spritePosition,
                        SpriteBounds = spriteBounds,
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
                        SpriteOrigin = new Vector2(0, spriteBounds.Y)
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
                        SpriteOrigin = new Vector2(0, spriteBounds.Y),
                        RelativeBlockingRectangle = new Rect(0, -spriteBounds.Y, spriteBounds.X, spriteBounds.Y),
                        RelativeHitboxRectangle = new Rect(0, -spriteBounds.Y, spriteBounds.X, spriteBounds.Y)
                    };
                    break;
                case PlacementType.LivingEntityPlacement:
                    selectedObject = new LivingEntityType()
                    {
                        ID = newId,
                        Name = "LivingEntity" + newId,
                        SpriteOrigin = new Point(0, spriteBounds.Y)
                    };
                    break;
            }

            string selectedObjectString;

            if(showDialog)
            {
                var dialog = new InputDialog("Create Object", JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString(Formatting.Indented));

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedObjectString = dialog.ResultText;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                selectedObjectString = JToken.FromObject(selectedObject, SerializationUtils.Serializer).ToString();
            }

            if (ReplaceTypeFromString(placementType, selectedObjectString) == false)
                return -1;

            return newId;
        }

        public static void DrawPreview(SpriteBatch spriteBatch, Vector2 position, MetaDataType obj)
        {
            if(obj is BlockType)
            {
                var blockType = (BlockType)obj;

                if (blockType.SpritePath != null)
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(blockType.SpritePath),
                        position, new Rectangle(blockType.SpritePosition, blockType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if(obj is AmbientObjectType)
            {
                var ambientObjectType = (AmbientObjectType)obj;

                if (ambientObjectType.SpritePath != null)
                {
                    var realPosition = new Vector2(position.X - ambientObjectType.SpriteOrigin.X, position.Y - ambientObjectType.SpriteOrigin.Y);

                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath),
                        realPosition, new Rectangle(ambientObjectType.SpritePositions[0], ambientObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
            else if(obj is AmbientHitableObjectType)
            {
                var ambientHitableObjectType = (AmbientHitableObjectType)obj;

                if (ambientHitableObjectType.SpritePath != null)
                {
                    var realPosition = new Vector2(position.X - ambientHitableObjectType.SpriteOrigin.X, position.Y - ambientHitableObjectType.SpriteOrigin.Y);

                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientHitableObjectType.SpritePath),
                        realPosition, new Rectangle(ambientHitableObjectType.SpritePositions[0], ambientHitableObjectType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
            else if(obj is LivingEntityType)
            {
                var livingEntityType = (LivingEntityType)obj;

                if (livingEntityType.SpritePath != null)
                {
                    var realPosition = new Vector2(position.X - livingEntityType.SpriteOrigin.X, position.Y - livingEntityType.SpriteOrigin.Y);

                    if (livingEntityType.WithGrid)
                    {
                        spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(livingEntityType.SpritePath),
                            realPosition, new Rectangle(new Point(livingEntityType.DownAnimation[0].X * livingEntityType.SpriteBounds.X, livingEntityType.DownAnimation[0].Y * livingEntityType.SpriteBounds.Y), livingEntityType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(livingEntityType.SpritePath),
                            realPosition, new Rectangle(livingEntityType.DownAnimation[0], livingEntityType.SpriteBounds), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }
                }
            }
        }

        public static bool ReplaceTypeFromString(PlacementType placementType, string objectText)
        {
            try
            {
                InputDialog dialog;

                switch (placementType)
                {
                    case PlacementType.BlockPlacement:
                        BlockType newBlockType = JsonConvert.DeserializeObject<BlockType>(objectText, SerializationUtils.SerializerSettings);
                        BlockType.lookup[newBlockType.ID] = newBlockType;
                        break;
                    case PlacementType.AmbientObjectPlacement:
                        AmbientObjectType newAmbientObjectType = JsonConvert.DeserializeObject<AmbientObjectType>(objectText, SerializationUtils.SerializerSettings);
                        AmbientObjectType.lookup[newAmbientObjectType.ID] = newAmbientObjectType;
                        break;
                    case PlacementType.AmbientHitableObjectPlacement:
                        AmbientHitableObjectType newAmbientHitableObjectType = JsonConvert.DeserializeObject<AmbientHitableObjectType>(objectText, SerializationUtils.SerializerSettings);
                        AmbientHitableObjectType.lookup[newAmbientHitableObjectType.ID] = newAmbientHitableObjectType;
                        break;
                    case PlacementType.LivingEntityPlacement:
                        dialog = new InputDialog("Create LivingEntity", objectText);

                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            LivingEntityType newLivingEntityType = JsonConvert.DeserializeObject<LivingEntityType>(dialog.ResultText, SerializationUtils.SerializerSettings);
                            LivingEntityType.lookup[newLivingEntityType.ID] = newLivingEntityType;

                            return true;
                        }

                        return false;
                }

                return true;
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error while replacing type: " + e.Message);

                return false;
            }
        }
    }
}
