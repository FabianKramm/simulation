using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects.Entities;
using Simulation.Util;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;

namespace Simulation.Game.Renderer.Entities
{
    class MovingEntityRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, MovingEntity movingEntity)
        {
            WalkingDirection newWalkingDirection = MovementUtils.GetWalkingDirectionFromVector(movingEntity.Direction);

            movingEntity.RendererInformation.Update(gameTime, newWalkingDirection);

            spriteBatch.Draw(movingEntity.RendererInformation.currentAnimation, new Vector2((int)movingEntity.Position.X, (int)movingEntity.Position.Y), color: GameRenderer.BlendColor, layerDepth: GeometryUtils.GetLayerDepthFromPosition(movingEntity.Position.X, movingEntity.Position.Y));
        }
    }
}
