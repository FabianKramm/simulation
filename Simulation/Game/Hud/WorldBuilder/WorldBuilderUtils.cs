using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using Simulation.Util.UI;

namespace Simulation.Game.Hud.WorldBuilder
{
    public static class WorldBuilderUtils
    {
        public static void CreateObjectAtMousePosition(UIElement listItem)
        {
            var realPosition = SimulationGame.RealWorldMousePosition;

            if (listItem is BlockListItem)
            {
                var blockPosition = GeometryUtils.GetBlockFromReal((int)realPosition.X, (int)realPosition.Y);

                SimulationGame.World.SetBlockType(blockPosition.X, blockPosition.Y, SimulationGame.Player.InteriorID, ((BlockListItem)listItem).BlockType.ID);
            }
            else
            {
                if (SimulationGame.KeyboardState.IsKeyDown(Keys.LeftControl) || SimulationGame.KeyboardState.IsKeyDown(Keys.RightControl))
                {
                    var blockPosition = GeometryUtils.GetBlockFromReal((int)realPosition.X, (int)realPosition.Y);

                    realPosition = new Vector2(blockPosition.X * WorldGrid.BlockSize.X, blockPosition.Y * WorldGrid.BlockSize.Y);
                }

                var newWorldPosition = new WorldPosition(realPosition.X, realPosition.Y, SimulationGame.Player.InteriorID);
                GameObject gameObject = null;

                if (listItem is AmbientObjectListItem)
                {
                    gameObject = AmbientObjectType.Create(newWorldPosition, ((AmbientObjectListItem)listItem).AmbientObjectType);
                }
                else if (listItem is AmbientHitableObjectListItem)
                {
                    gameObject = AmbientHitableObjectType.Create(newWorldPosition, ((AmbientHitableObjectListItem)listItem).AmbientHitableObjectType);
                }
                else if (listItem is LivingEntityListItem)
                {
                    gameObject = LivingEntityType.Create(newWorldPosition, ((LivingEntityListItem)listItem).LivingEntityType);
                }

                gameObject.ConnectToWorld();
            }
        } 

        public static int GenerateNewId(WorldBuilder.PlacementType placementType)
        {
            int highestNumber = int.MinValue;

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

        public static void ReplaceTypeFromString(WorldBuilder.PlacementType placementType, string objectText)
        {
            switch (placementType)
            {
                case WorldBuilder.PlacementType.BlockPlacement:
                    BlockType newBlockType = JsonConvert.DeserializeObject<BlockType>(objectText, SerializationUtils.SerializerSettings);
                    BlockType.lookup[newBlockType.ID] = newBlockType;
                    break;
                case WorldBuilder.PlacementType.AmbientObjectPlacement:
                    AmbientObjectType newAmbientObjectType = JsonConvert.DeserializeObject<AmbientObjectType>(objectText, SerializationUtils.SerializerSettings);
                    AmbientObjectType.lookup[newAmbientObjectType.ID] = newAmbientObjectType;
                    break;
                case WorldBuilder.PlacementType.AmbientHitableObjectPlacement:
                    AmbientHitableObjectType newAmbientHitableObjectType = JsonConvert.DeserializeObject<AmbientHitableObjectType>(objectText, SerializationUtils.SerializerSettings);
                    AmbientHitableObjectType.lookup[newAmbientHitableObjectType.ID] = newAmbientHitableObjectType;
                    break;
                case WorldBuilder.PlacementType.LivingEntityPlacement:
                    LivingEntityType newLivingEntityType = JsonConvert.DeserializeObject<LivingEntityType>(objectText, SerializationUtils.SerializerSettings);
                    LivingEntityType.lookup[newLivingEntityType.ID] = newLivingEntityType;
                    break;
            }
        }
    }
}
