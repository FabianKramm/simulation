using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.World;

namespace Simulation.Game.Base
{
    public abstract class HitableObject: DrawableObject
    {
        private Rectangle relativeHitBoxBounds;
        private Rectangle relativeBlockingBounds;

        public bool useSameBounds
        {
            get; private set;
        }

        public BlockingType blockingType
        {
            get; private set;
        }

        public Rectangle hitBoxBounds;
        public Rectangle blockingBounds;

        public HitableObject(Vector2 position, Rectangle relativeHitBoxBounds, BlockingType blockingType = BlockingType.NOT_BLOCKING, Rectangle? relativeBlockingBounds = null) : base(position)
        {
            this.blockingType = blockingType;
            this.relativeHitBoxBounds = relativeHitBoxBounds;
            useSameBounds = false;

            if (blockingType == BlockingType.BLOCKING)
            {
                if (relativeBlockingBounds != null)
                {
                    this.relativeBlockingBounds = relativeBlockingBounds ?? Rectangle.Empty;
                }
                else
                {
                    useSameBounds = true;
                }
            }

            updateHitableBounds(position);
        }

        private void updateHitableBounds(Vector2 newPosition)
        {
            hitBoxBounds = new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            blockingBounds = useSameBounds ? hitBoxBounds : new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);
        }
        
        public bool canMove(Vector2 newPosition)
        {
            if(blockingType == BlockingType.NOT_BLOCKING)
            {
                return SimulationGame.world.canMove(new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return SimulationGame.world.canMove(useSameBounds ?
                    new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        public override void updatePosition(Vector2 newPosition)
        {
            base.updatePosition(newPosition);

            updateHitableBounds(position);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (SimulationGame.isDebug)
            {
                if(blockingType == BlockingType.BLOCKING)
                {
                    SimulationGame.primitiveDrawer.Rectangle(hitBoxBounds, Color.Black);
                    SimulationGame.primitiveDrawer.Rectangle(blockingBounds, Color.Red);
                }
                else
                {
                    SimulationGame.primitiveDrawer.Rectangle(hitBoxBounds, Color.Red);
                }
            }
        }
    }
}
