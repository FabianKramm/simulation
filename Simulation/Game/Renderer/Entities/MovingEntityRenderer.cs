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
            WalkingDirection newWalkingDirection = Movement.getWalkingDirectionFromVector(movingEntity.direction);

            movingEntity.rendererInformation.Update(gameTime, newWalkingDirection);

            spriteBatch.Draw(movingEntity.rendererInformation.currentAnimation, movingEntity.position, layerDepth: GeometryUtils.getLayerDepthFromPosition(movingEntity.position.X, movingEntity.position.Y));
        }
    }
}
