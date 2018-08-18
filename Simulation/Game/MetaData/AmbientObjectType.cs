using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Spritesheet;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientObjectType: MetaDataType
    {
        public static Dictionary<int, AmbientObjectType> lookup = new Dictionary<int, AmbientObjectType>() {};
        
        // Render
        public string SpritePath;
        public Vector2 SpriteOrigin;
        public Point SpriteBounds;
        public Point[] SpritePositions;
        public bool HasDepth = true;
        public bool InForeground = false;

        public string CustomRendererScript = null;
        public string CustomControllerScript = null;
        public JObject CustomProperties = null;

        public int FrameDuration = 180;
        public int LiveSpan = -1;

        public static AmbientObject Create(WorldPosition worldPosition, AmbientObjectType ambientObjectType)
        {
            var ambientObject = new AmbientObject(worldPosition)
            {
                AmbientObjectType = ambientObjectType.ID,
                CustomProperties = ambientObjectType.CustomProperties != null ? (JObject)ambientObjectType.CustomProperties.DeepClone() : null,
                YPositionDepthOffset = ambientObjectType.YPositionDepthOffset,
                LiveSpan = ambientObjectType.LiveSpan,
            };

            ambientObject.Init();

            return ambientObject;
        }

        public static Animation CreateAnimation(AmbientObject ambientObject)
        {
            var ambientObjectType = lookup[ambientObject.AmbientObjectType];

            var texture = SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath);
            var frames = new Frame[ambientObjectType.SpritePositions.Length];

            for (var i = 0; i < ambientObjectType.SpritePositions.Length; i++)
                frames[i] = new Frame(texture, new Rectangle(ambientObjectType.SpritePositions[i], ambientObjectType.SpriteBounds), ambientObjectType.SpriteOrigin.ToPoint(), ambientObjectType.FrameDuration, SpriteEffects.None);

            return new Animation(frames);
        }
    }
}
