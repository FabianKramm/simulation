﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Fractions;
using Simulation.Game.MetaData;
using Simulation.Game.Objects;
using Simulation.Game.World;
using Simulation.Spritesheet;
using Simulation.Util;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class AmbientHitableObjectRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, AmbientHitableObject ambientHitableObject)
        {
            if (ambientHitableObject is AmbientHitableObject)
            {
                var ambientHitableObjectType = AmbientHitableObjectType.lookup[((AmbientHitableObject)ambientHitableObject).AmbientHitableObjectType];

                if (ambientHitableObjectType.SpritePositions.Length > 1)
                {
                    if (ambientHitableObject.ObjectAnimation == null)
                    {
                        ambientHitableObject.ObjectAnimation = AmbientHitableObjectType.CreateAnimation(ambientHitableObject);
                        ambientHitableObject.ObjectAnimation.Start(Repeat.Mode.Loop);
                    }
                        

                    ambientHitableObject.ObjectAnimation.Update(gameTime);
                    spriteBatch.Draw(ambientHitableObject.ObjectAnimation, ambientHitableObject.Position.ToVector(), color: Color.White, layerDepth: GeometryUtils.GetLayerDepthFromPosition(ambientHitableObject.Position.X, ambientHitableObject.Position.Y + ambientHitableObject.YPositionDepthOffset));
                }
                else
                {
                    spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(ambientHitableObjectType.SpritePath), ambientHitableObject.Position.ToVector(), new Rectangle(ambientHitableObjectType.SpritePositions[0], ambientHitableObjectType.SpriteBounds), Color.White, 0.0f, ambientHitableObjectType.SpriteOrigin, 1.0f, SpriteEffects.None, GeometryUtils.GetLayerDepthFromPosition(ambientHitableObject.Position.X, ambientHitableObject.Position.Y + ambientHitableObject.YPositionDepthOffset));
                }

                if (SimulationGame.IsDebug)
                {
                    if (ambientHitableObject.IsBlocking())
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
