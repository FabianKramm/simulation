using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.MetaData
{
    public class AmbientObjectType
    {
        public static readonly Dictionary<int, AmbientObjectType> lookup = new Dictionary<int, AmbientObjectType>() {
            { 0, new AmbientObjectType()
            {
                ID=0,
                Name="SMALL_ROCK01",
                SpritePath=@"Environment\Rock01",
                SpriteOrigin=new Vector2(0, 20),
                SpriteBounds=new Point(25, 20),
                SpritePositions=new Point[] {new Point(0,0)}
            }},
            { 1, new AmbientObjectType()
            {
                ID=1,
                Name="SMALL_ROCK02",
                SpritePath=@"Environment\Rock02",
                SpriteOrigin=new Vector2(0, 20),
                SpriteBounds=new Point(25, 20),
                SpritePositions=new Point[] {new Point(0,0)}
            }}
        };

        public int ID;
        public string Name;

        // Render
        public string SpritePath;
        public Vector2 SpriteOrigin;
        public Point SpriteBounds;
        public Point[] SpritePositions;
        public bool HasDepth = false;

        public static AmbientObject Create(WorldPosition worldPosition, AmbientObjectType ambientObjectType)
        {
            var ambientObject = new AmbientObject(worldPosition)
            {
                AmbientObjectType = ambientObjectType.ID
            };

            ambientObject.Init();

            return ambientObject;
        }
    }
}
