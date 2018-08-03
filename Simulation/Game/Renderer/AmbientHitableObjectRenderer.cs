using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class AmbientHitableObjectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, HitableObject ambientHitableObject)
        {
            if (ambientHitableObject is AmbientHitableObject)
            {
                var ambientHitableObjectType = AmbientHitableObjectType.lookup[((AmbientHitableObject)ambientHitableObject).AmbientHitableObjectType];

                spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientHitableObjectType.SpritePath), ambientHitableObject.Position.ToVector(), new Rectangle(ambientHitableObjectType.SpritePositions[0], ambientHitableObjectType.SpriteBounds), GameRenderer.BlendColor, 0.0f, ambientHitableObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, GeometryUtils.GetLayerDepthFromPosition(ambientHitableObject.Position.X, ambientHitableObject.Position.Y));
                
                if (SimulationGame.IsDebug)
                {
                    if (ambientHitableObject.BlockingType == BlockingType.BLOCKING)
                    {
                        SimulationGame.PrimitiveDrawer.Rectangle(ambientHitableObject.UnionBounds.ToXnaRectangle(), Color.Red);
                    }
                    else
                    {
                        SimulationGame.PrimitiveDrawer.Rectangle(ambientHitableObject.HitBoxBounds.ToXnaRectangle(), Color.White);
                    }
                }
            }
        }
    }
}
