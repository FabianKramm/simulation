using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.World;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public enum InteractiveObjectType
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

        private static Dictionary<InteractiveObjectType, InteractiveObjectRenderInformation> interactiveObjectLookup = new Dictionary<InteractiveObjectType, InteractiveObjectRenderInformation> {
            { InteractiveObjectType.NO_OBJECT, null },
            { InteractiveObjectType.TREE01, new InteractiveObjectRenderInformation(@"Environment\Tree01", new Rectangle(0, 0, 79, 91)) }
        };

        public static void Draw(SpriteBatch spriteBatch, HitableObject interactiveObject)
        {
            var renderInformation = interactiveObjectLookup[interactiveObject.interactiveObjectType];

            if (renderInformation != null)
            {
                spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(renderInformation.texture), interactiveObject.position, renderInformation.spriteRectangle, Color.White, 0.0f, renderInformation.origin, 1.0f, SpriteEffects.None, GeometryUtils.getLayerDepthFromPosition(interactiveObject.position.X, interactiveObject.position.Y));
            }

            if (SimulationGame.isDebug)
            {
                if (interactiveObject.blockingType == BlockingType.BLOCKING)
                {
                    SimulationGame.primitiveDrawer.Rectangle(interactiveObject.unionBounds, Color.Red);
                }
                else
                {
                    SimulationGame.primitiveDrawer.Rectangle(interactiveObject.hitBoxBounds, Color.White);
                }
            }

        }
    }
}
