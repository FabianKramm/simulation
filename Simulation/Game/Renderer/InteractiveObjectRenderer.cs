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
    public class InteractiveObjectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, HitableObject interactiveObject)
        {
            if (SimulationGame.VisibleArea.Contains(interactiveObject.Position) && interactiveObject.InteriorID == SimulationGame.Player.InteriorID)
            {
                if (interactiveObject is AmbientHitableObject)
                {
                    var ambientHitableObjectType = AmbientHitableObjectType.lookup[((AmbientHitableObject)interactiveObject).AmbientHitableObjectType];

                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientHitableObjectType.SpritePath), interactiveObject.Position.ToVector(), new Rectangle(ambientHitableObjectType.SpritePositions[0], ambientHitableObjectType.SpriteBounds), GameRenderer.BlendColor, 0.0f, ambientHitableObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, GeometryUtils.GetLayerDepthFromPosition(interactiveObject.Position.X, interactiveObject.Position.Y));
                
                    if (SimulationGame.IsDebug)
                    {
                        if (interactiveObject.BlockingType == BlockingType.BLOCKING)
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(interactiveObject.UnionBounds.ToXnaRectangle(), Color.Red);
                        }
                        else
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(interactiveObject.HitBoxBounds.ToXnaRectangle(), Color.White);
                        }
                    }
                }
            }
        }
    }
}
