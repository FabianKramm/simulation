using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public enum AmbientHitableObjectType
    {
        NO_OBJECT = 0,
        TREE01,
    }

    public class InteractiveObjectRenderer
    {
        private class InteractiveObjectRenderInformation
        {
            public string texture
            {
                get; private set;
            }

            public Rectangle spriteRectangle
            {
                get; private set;
            }

            public Vector2 origin
            {
                get; private set;
            }

            public InteractiveObjectRenderInformation(string texture, Rectangle spriteRectangle)
            {
                this.texture = texture;
                this.spriteRectangle = spriteRectangle;

                origin = new Vector2(0, spriteRectangle.Height);
            }
        }

        private static Dictionary<AmbientHitableObjectType, InteractiveObjectRenderInformation> ambientHitableObjectLookup = new Dictionary<AmbientHitableObjectType, InteractiveObjectRenderInformation> {
            { AmbientHitableObjectType.NO_OBJECT, null },
            { AmbientHitableObjectType.TREE01, new InteractiveObjectRenderInformation(@"Environment\Tree01", new Rectangle(0, 0, 79, 91)) }
        };

        public static void Draw(SpriteBatch spriteBatch, HitableObject interactiveObject)
        {
            if (SimulationGame.VisibleArea.Contains(interactiveObject.Position) && interactiveObject.InteriorID == SimulationGame.Player.InteriorID)
            {
                if (interactiveObject is AmbientHitableObject)
                {
                    var renderInformation = ambientHitableObjectLookup[((AmbientHitableObject)interactiveObject).AmbientHitableObjectType];

                    if (renderInformation != null)
                    {
                        spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(renderInformation.texture), interactiveObject.Position, renderInformation.spriteRectangle, GameRenderer.BlendColor, 0.0f, renderInformation.origin, 1.0f, SpriteEffects.None, GeometryUtils.GetLayerDepthFromPosition(interactiveObject.Position.X, interactiveObject.Position.Y));
                    }

                    if (SimulationGame.IsDebug)
                    {
                        if (interactiveObject.BlockingType == BlockingType.BLOCKING)
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(interactiveObject.UnionBounds.ToRectangle(), Color.Red);
                        }
                        else
                        {
                            SimulationGame.PrimitiveDrawer.Rectangle(interactiveObject.HitBoxBounds.ToRectangle(), Color.White);
                        }
                    }
                }
            }
        }
    }
}
