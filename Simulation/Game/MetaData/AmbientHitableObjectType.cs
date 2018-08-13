using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientHitableObjectType: MetaDataType
    {
        public static Dictionary<int, AmbientHitableObjectType> lookup = new Dictionary<int, AmbientHitableObjectType>();

        // General
        public int ID;
        public string Name;

        public Rect RelativeBlockingRectangle;
        public Rect RelativeHitboxRectangle;

        public bool IsHitable = true;
        public BlockingType BlockingType = BlockingType.BLOCKING;

        // Render
        public string SpritePath;
        public Vector2 SpriteOrigin;
        public Point SpriteBounds;
        public Point[] SpritePositions;

        public string CustomRendererScript = null;
        public string CustomControllerScript = null;
        public JObject CustomProperties = null;

        public static AmbientHitableObject Create(WorldPosition worldPosition, AmbientHitableObjectType ambientHitableObjectType)
        {
            AmbientHitableObject ambientHitableObject = new AmbientHitableObject(worldPosition)
            {
                AmbientHitableObjectType = ambientHitableObjectType.ID,
                BlockingType=ambientHitableObjectType.BlockingType,
                IsHitable=ambientHitableObjectType.IsHitable,
                CustomProperties= ambientHitableObjectType.CustomProperties != null ? (JObject)ambientHitableObjectType.CustomProperties.DeepClone() : null
            };

            ambientHitableObject.Init();

            return ambientHitableObject;
        }
    }
}
