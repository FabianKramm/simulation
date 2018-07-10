using Newtonsoft.Json.Linq;
using Simulation.Game.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization.AI
{
    public abstract class BaseAISerializer
    {
        private static readonly Type baseAiType = typeof(BaseAI);
        private static readonly string[] serializeableProperties = new string[] {  };

        protected static void Deserialize(ref JObject jObject, BaseAI baseAI)
        {
            SerializationUtils.SetFromObject(jObject, baseAI, baseAiType, serializeableProperties);
        }

        protected static void Serialize(BaseAI baseAI, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, baseAI, baseAiType, serializeableProperties);
        }
    }
}
