using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Objects.Interfaces;
using Simulation.Game.Serialization.Skills;
using Simulation.Game.Skills;
using System;

namespace Simulation.Game.Serialization.Objects
{
    public class LivingEntitySerializer: HitableObjectSerializer
    {
        private static readonly Type type = typeof(LivingEntity);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(type);

        protected static void Deserialize(ref JObject jObject, LivingEntity livingEntity)
        {
            HitableObjectSerializer.Deserialize(ref jObject, livingEntity);

            SerializationUtils.SetFromObject(jObject, livingEntity, type, serializeableProperties);

            if(jObject.GetValue("Skills") != null)
            {
                // Deserialize Skills
                JArray skills = (JArray)jObject.GetValue("Skills");
                Skill[] skillObjects = new Skill[skills.Count];

                for (int i=0;i<skills.Count;i++)
                    skillObjects[i] = ((Skill)BaseSkillSerializer.Deserialize((JObject)skills[i], livingEntity));

                livingEntity.Skills = skillObjects;
            }

            var livingEntityType = LivingEntityType.lookup[livingEntity.LivingEntityType];

            if(livingEntityType.CustomControllerAssembly != null)
            {
                livingEntity.CustomController = (GameObjectController)SerializationUtils.GetAssembly(livingEntityType.CustomControllerAssembly).GetType("CustomController").GetMethod("Create").Invoke(null, new object[] { livingEntity });
            }

            if (livingEntityType.CustomRendererAssembly != null)
            {
                livingEntity.CustomRenderer = (GameObjectRenderer)SerializationUtils.GetAssembly(livingEntityType.CustomRendererAssembly).GetType("CustomRenderer").GetMethod("Create").Invoke(null, new object[] { livingEntity });
            }
        }

        protected static void Serialize(LivingEntity livingEntity, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(livingEntity, ref jObject);

            SerializationUtils.AddToObject(jObject, livingEntity, type, serializeableProperties);

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
