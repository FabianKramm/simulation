using Simulation.Game.AI;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;

namespace Simulation.Game.MetaData.AI
{
    public abstract class AIMetaData
    {
        private static Dictionary<string, Func<MovingEntity, AIMetaData, BaseAI>> aiCreatorLookup = new Dictionary<string, Func<MovingEntity, AIMetaData, BaseAI>>()
        {
            {typeof(WanderAIMetaData).FullName, (MovingEntity movingEntity, AIMetaData aiMetaData) => WanderAIMetaData.Create(movingEntity, (WanderAIMetaData)aiMetaData)}
        };

        public static BaseAI Create(MovingEntity movingEntity, AIMetaData aiMetaData) => aiCreatorLookup[aiMetaData.GetType().FullName](movingEntity, aiMetaData);
    }
}
