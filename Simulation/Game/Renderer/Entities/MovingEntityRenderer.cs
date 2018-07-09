using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base.Entity;
using Simulation.Util;
using Simulation.Spritesheet;

namespace Simulation.Game.Renderer.Entities
{
    class MovingEntityRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, MovingEntity movingEntity)
        {
            WalkingDirection newWalkingDirection = Movement.getWalkingDirectionFromVector(movingEntity.Direction);

            movingEntity.RendererInformation.Update(gameTime, newWalkingDirection);

            spriteBatch.Draw(movingEntity.RendererInformation.currentAnimation, movingEntity.Position, color: GameRenderer.BlendColor, layerDepth: GeometryUtils.getLayerDepthFromPosition(movingEntity.Position.X, movingEntity.Position.Y));
        }
    }
}
