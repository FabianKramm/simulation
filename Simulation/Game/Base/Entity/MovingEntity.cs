using Microsoft.Xna.Framework;
using Simulation.Game.Renderer.Entities;

namespace Simulation.Game.Base.Entity
{
    public class MovingEntity: LivingEntity
    {
        public Vector2 direction;
        private float velocity = 0.3f;

        // Create from JSON
        protected MovingEntity() { }

        public MovingEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds) :
            base(livingEntityType, position, relativeHitBoxBounds)
        {

        }

        public override void updatePosition(Vector2 newPosition)
        {
            if (canMove(newPosition))
            {
                SimulationGame.world.removeInteractiveObject(this);

                base.updatePosition(newPosition);

                SimulationGame.world.addInteractiveObject(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(direction != Vector2.Zero)
            {
                // TODO: check if we walk out of loaded bounds

                updatePosition(new Vector2(position.X + direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds, position.Y + direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds));
            }
        }
    }
}
