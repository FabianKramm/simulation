using Newtonsoft.Json.Linq;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Interfaces;
using Simulation.Util;
using System;
using System.IO;

namespace Simulation.Game.Serialization.Objects
{
    public class AmbientHitableObjectSerializer: HitableObjectSerializer
    {
        private static readonly Type type = typeof(AmbientHitableObject);
        private static readonly string[] serializeableProperties = SerializationUtils.GetSerializeables(type);

        public static AmbientHitableObject Deserialize(JObject jObject)
        {
            AmbientHitableObject staticBlockingObject = ReflectionUtils.CallPrivateConstructor<AmbientHitableObject>();

            Deserialize(ref jObject, staticBlockingObject);

            return staticBlockingObject;
        }

        public static JObject Serialize(AmbientHitableObject staticBlockingObject)
        {
            var retObject = new JObject();

            SerializationUtils.SerializeType(type, ref retObject);
            Serialize(staticBlockingObject, ref retObject);

            return retObject;
        }

        protected static void Deserialize(ref JObject jObject, AmbientHitableObject ambientHitableObject)
        {
            HitableObjectSerializer.Deserialize(ref jObject, ambientHitableObject);

            SerializationUtils.SetFromObject(jObject, ambientHitableObject, type, serializeableProperties);

            var ambientHitableObjectType = AmbientHitableObjectType.lookup[ambientHitableObject.AmbientHitableObjectType];

            if (ambientHitableObjectType.CustomControllerAssembly != null)
            {
                ambientHitableObject.CustomController = (GameObjectController)SerializationUtils
                    .GetAssembly(ambientHitableObjectType.CustomControllerAssembly)
                    .CreateInstance(Path.GetFileNameWithoutExtension(ambientHitableObjectType.CustomControllerAssembly));
            }

            if (ambientHitableObjectType.CustomRendererAssembly != null)
            {
                ambientHitableObject.CustomRenderer = (GameObjectRenderer)SerializationUtils
                    .GetAssembly(ambientHitableObjectType.CustomRendererAssembly)
                    .CreateInstance(Path.GetFileNameWithoutExtension(ambientHitableObjectType.CustomRendererAssembly));
            }

            ambientHitableObject.Init();
        }

        protected static void Serialize(AmbientHitableObject staticBlockingObject, ref JObject jObject)
        {
            HitableObjectSerializer.Serialize(staticBlockingObject, ref jObject);

            SerializationUtils.AddToObject(jObject, staticBlockingObject, type, serializeableProperties);
        }
    }
}
