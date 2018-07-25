using Newtonsoft.Json.Linq;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization.AI;
using Simulation.Game.Serialization.Skills;
using Simulation.Game.Skills;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class LivingEntitySerializer: HitableObjectSerializer
    {
        private static readonly Type livingEntityType = typeof(LivingEntity);
        private static readonly string[] serializeableProperties = new string[] {
            "LivingEntityType",
            "Fraction",
            "MaximumLife",
            "CurrentLife",
            "AttentionRadius"
        };

        protected static void Deserialize(ref JObject jObject, LivingEntity livingEntity)
        {
            HitableObjectSerializer.Deserialize(ref jObject, livingEntity);

            SerializationUtils.SetFromObject(jObject, livingEntity, livingEntityType, serializeableProperties);

            if(jObject.GetValue("BaseAI") != null)
            {
                livingEntity.BaseAI = AISerializer.Deserialize((JObject)jObject.GetValue("BaseAI"), livingEntity);
                livingEntity.BaseAI.Init(); // Important to create behaviortree
            }

            if(jObject.GetValue("Skills") != null)
            {
                // Deserialize Skills
                JArray skills = (JArray)jObject.GetValue("Skills");
                Skill[] skillObjects = new Skill[skills.Count];

                for (int i=0;i<skills.Count;i++)
                    skillObjects[i] = ((Skill)BaseSkillSerializer.Deserialize((JObject)skills[i], livingEntity));

                livingEntity.Skills = skillObjects;
            }
        }

        protected static void Serialize(LivingEntity livingEntity, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(livingEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, livingEntity, livingEntityType, serializeableProperties);

            jObject.Add("BaseAI", livingEntity.BaseAI != null ? AISerializer.Serialize(livingEntity.BaseAI) : null);

            // Serialize skills
            if(livingEntity.Skills != null)
            {
                JArray skills = new JArray();
                
                foreach (var skill in livingEntity.Skills)
                    skills.Add(BaseSkillSerializer.Serialize(skill));

                jObject.Add("Skills", skills);
            }
        }
    }
}
