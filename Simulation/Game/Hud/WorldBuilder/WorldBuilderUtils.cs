using Newtonsoft.Json;
using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Hud.WorldBuilder
{
    public static class WorldBuilderUtils
    {
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
