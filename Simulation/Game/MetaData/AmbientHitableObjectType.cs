using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientHitableObjectType
    {
        public static readonly Dictionary<int, AmbientHitableObjectType> lookup = new Dictionary<int, AmbientHitableObjectType>()
        {
            {0, new AmbientHitableObjectType()
            {
                ID=0,
                Name="Tree_01",
                RelativeBlockingRectangle=new Rect(6, -36, 67, 36),
                RelativeHitboxRectangle=new Rect(6, -36, 67, 36),
                SpritePath=@"Environment\Tree01",
                SpritePositions=new Point[] {Point.Zero},
                SpriteBounds=new Point(79, 91),
                SpriteOrigin=new Vector2(0, 91)
            }}
        };

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

        public static AmbientHitableObject Create(WorldPosition worldPosition, AmbientHitableObjectType ambientHitableObjectType)
        {
            AmbientHitableObject ambientHitableObject = new AmbientHitableObject(worldPosition)
            {
                AmbientHitableObjectType = ambientHitableObjectType.ID,
                RelativeHitBoxBounds = ambientHitableObjectType.RelativeHitboxRectangle,
                RelativeBlockingBounds = ambientHitableObjectType.RelativeBlockingRectangle,
                BlockingType=ambientHitableObjectType.BlockingType,
                IsHitable=ambientHitableObjectType.IsHitable
            };

            ambientHitableObject.Init();

            return ambientHitableObject;
        }
    }
}
