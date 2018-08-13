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

        public int FrameDuration = 120;

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

        public static Animation CreateAnimation(AmbientObject ambientObject)
        {
            var ambientObjectType = lookup[ambientObject.AmbientObjectType];

            var texture = SimulationGame.ContentManager.Load<Texture2D>(ambientObjectType.SpritePath);
            var sheet = new Spritesheet.Spritesheet(texture);

            sheet = sheet.WithCellOrigin(ambientObjectType.SpriteOrigin.ToPoint()).WithFrameDuration(ambientObjectType.FrameDuration);

            Frame[] frames = new Frame[ambientObjectType.SpritePositions.Length];

            for (var i = 0; i < ambientObjectType.SpritePositions.Length; i++)
                frames[i] = sheet.CreateFrame(ambientObjectType.SpritePositions[i].X, ambientObjectType.SpritePositions[i].Y, sheet.FrameDefaultDuration, sheet.FrameDefaultEffects);

            return new Animation(frames);
        }
    }
}
