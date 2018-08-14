using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientHitableObjectType: MetaDataType
    {
        public static Dictionary<int, AmbientHitableObjectType> lookup = new Dictionary<int, AmbientHitableObjectType>();

        // General
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

        public int FrameDuration = 180;

        public static AmbientHitableObject Create(WorldPosition worldPosition, AmbientHitableObjectType ambientHitableObjectType)
        {
            AmbientHitableObject ambientHitableObject = new AmbientHitableObject(worldPosition)
            {
                AmbientHitableObjectType = ambientHitableObjectType.ID,
                BlockingType=ambientHitableObjectType.BlockingType,
                IsHitable=ambientHitableObjectType.IsHitable,
                CustomProperties= ambientHitableObjectType.CustomProperties != null ? (JObject)ambientHitableObjectType.CustomProperties.DeepClone() : null,
                YPositionDepthOffset = ambientHitableObjectType.YPositionDepthOffset,
            };

            ambientHitableObject.Init();

            return ambientHitableObject;
        }

        public static Animation CreateAnimation(AmbientHitableObject ambientHitableObject)
        {
            var ambientHitableObjectType = lookup[ambientHitableObject.AmbientHitableObjectType];

            var texture = SimulationGame.ContentManager.Load<Texture2D>(ambientHitableObjectType.SpritePath);
            var frames = new Frame[ambientHitableObjectType.SpritePositions.Length];

            for (var i = 0; i < ambientHitableObjectType.SpritePositions.Length; i++)
                frames[i] = new Frame(texture, new Rectangle(ambientHitableObjectType.SpritePositions[i], ambientHitableObjectType.SpriteBounds), ambientHitableObjectType.SpriteOrigin.ToPoint(), ambientHitableObjectType.FrameDuration, SpriteEffects.None);

            return new Animation(frames);
        }
    }
}
