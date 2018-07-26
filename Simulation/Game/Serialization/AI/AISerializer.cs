using Newtonsoft.Json.Linq;
using Simulation.Game.AI;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;

namespace Simulation.Game.Serialization.AI
{
    public class AISerializer
    {
        private static Dictionary<string, Func<JObject, LivingEntity, BaseAI>> aiDeserializationLookup = new Dictionary<string, Func<JObject, LivingEntity, BaseAI>>()
        {
            {typeof(WanderAI).FullName, (JObject jObject, LivingEntity livingEntity) => WanderAISerializer.Deserialize(jObject, (MovingEntity)livingEntity)},
            {typeof(FollowAI).FullName, (JObject jObject, LivingEntity livingEntity) => throw new Exception("FollowAI cannot be deserialized!")}
        };

        private static Dictionary<string, Func<BaseAI, JObject>> aiSerializationLookup = new Dictionary<string, Func<BaseAI, JObject>>()
        {
            {typeof(WanderAI).FullName, (BaseAI baseAI) => WanderAISerializer.Serialize((WanderAI)baseAI)},
            {typeof(FollowAI).FullName, (BaseAI baseAI) => throw new Exception("FollowAI cannot be serialized!")}
        };

        public static JObject Serialize(BaseAI baseAI) => aiSerializationLookup[baseAI.GetType().FullName](baseAI);
        public static BaseAI Deserialize(JObject jObject, LivingEntity livingEntity) => aiDeserializationLookup[jObject.GetValue("type").ToString()](jObject, livingEntity);
    }
}
