﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Util;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public enum AmbientObjectType
    {
        NO_OBJECT = 0,
        SMALL_ROCK01,
        SMALL_ROCK02,
        SMALL_ROCK03,
        SMALL_ROCK04,
        SMALL_ROCK05
    }

    public class AmbientObjectRenderer
    {
        private class AmbientObjectRenderInformation
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

            public bool hasDepth
            {
                get; private set;
            }

            public AmbientObjectRenderInformation(string texture, Rectangle spriteRectangle, bool hasDepth = false)
            {
                this.texture = texture;
                this.spriteRectangle = spriteRectangle;
                this.hasDepth = hasDepth;

                origin = new Vector2(0, spriteRectangle.Height);
            }
        }

        private static Dictionary<AmbientObjectType, AmbientObjectRenderInformation> ambientObjectLookup = new Dictionary<AmbientObjectType, AmbientObjectRenderInformation> {
            { AmbientObjectType.NO_OBJECT, null },
            { AmbientObjectType.SMALL_ROCK01, new AmbientObjectRenderInformation(@"Environment\Rock01", new Rectangle(0, 0, 25, 20)) },
            { AmbientObjectType.SMALL_ROCK02, new AmbientObjectRenderInformation(@"Environment\Rock02", new Rectangle(0, 0, 25, 20)) },
            { AmbientObjectType.SMALL_ROCK03, new AmbientObjectRenderInformation(@"Environment\Rock03", new Rectangle(0, 0, 25, 20)) },
            { AmbientObjectType.SMALL_ROCK04, new AmbientObjectRenderInformation(@"Environment\Rock04", new Rectangle(0, 0, 25, 20)) },
            { AmbientObjectType.SMALL_ROCK05, new AmbientObjectRenderInformation(@"Environment\Rock05", new Rectangle(0, 0, 25, 20)) }
        };

        public static void Draw(SpriteBatch spriteBatch, AmbientObject ambientObject)
        {
            var renderInformation = ambientObjectLookup[ambientObject.ambientObjectType];

            if (renderInformation != null)
            {
                spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(renderInformation.texture), ambientObject.position, renderInformation.spriteRectangle, Color.White, 0.0f, renderInformation.origin, 1.0f, SpriteEffects.None, renderInformation.hasDepth ? GeometryUtils.getLayerDepthFromPosition(ambientObject.position.X, ambientObject.position.Y) : GeometryUtils.getLayerDepthFromReservedLayer(ReservedDepthLayers.BlockDecoration));
            }
        }
    }
}