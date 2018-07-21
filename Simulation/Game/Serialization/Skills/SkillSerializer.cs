using Newtonsoft.Json.Linq;
using Simulation.Game.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Serialization.Skills
{
    public abstract class SkillSerializer
    {
        private static readonly Type type = typeof(Skill);
        private static readonly string[] serializeableProperties = new string[] {
            "Cooldown"
        };

        protected static void Deserialize(ref JObject jObject, Skill deserialize)
        {
            SerializationUtils.SetFromObject(jObject, deserialize, type, serializeableProperties);
        }

        protected static void Serialize(Skill serialize, ref JObject jObject)
        {
            SerializationUtils.AddToObject(jObject, serialize, type, serializeableProperties);
        }
    }
}
