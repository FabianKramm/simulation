using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientObjectType: MetaDataType
    {
        public static Dictionary<int, AmbientObjectType> lookup = new Dictionary<int, AmbientObjectType>()
        {
            
        };

        public int ID;
        public string Name;

        // Render
        public string SpritePath;
        public Vector2 SpriteOrigin;
        public Point SpriteBounds;
        public Point[] SpritePositions;
        public bool HasDepth = true;

        public string CustomRendererScript = null;
        public string CustomControllerScript = null;
        public JObject CustomProperties = null;

        public static AmbientObject Create(WorldPosition worldPosition, AmbientObjectType ambientObjectType)
        {
            var ambientObject = new AmbientObject(worldPosition)
            {
                AmbientObjectType = ambientObjectType.ID,
                CustomProperties = ambientObjectType.CustomProperties != null ? (JObject)ambientObjectType.CustomProperties.DeepClone() : null
            };

            ambientObject.Init();

            return ambientObject;
        }
    }
}
