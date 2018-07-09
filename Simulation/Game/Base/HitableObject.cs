using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Base
{
    public abstract class HitableObject: DrawableObject
    {
        protected Rectangle relativeHitBoxBounds;
        protected Rectangle relativeBlockingBounds;

        public bool UseSameBounds
        {
            get; private set;
        }

        public BlockingType BlockingType
        {
            get; private set;
        }

        public Rectangle HitBoxBounds;
        public Rectangle BlockingBounds;

        public Rectangle UnionBounds;

        // Create from JSON
        protected HitableObject() { }

        public HitableObject(Vector2 position, Rectangle relativeHitBoxBounds, BlockingType blockingType = BlockingType.NOT_BLOCKING, Rectangle? relativeBlockingBounds = null)
            :base(position)
        {
            this.relativeHitBoxBounds = relativeHitBoxBounds;

            SetBlockingType(blockingType);
            updateHitableBounds(position);
        }

        public void SetBlockingType(BlockingType blockingType, Rectangle? relativeBlockingBounds = null)
        {
            this.BlockingType = blockingType;

            if (blockingType == BlockingType.BLOCKING)
            {
                if (relativeBlockingBounds != null)
                {
                    this.relativeBlockingBounds = relativeBlockingBounds ?? Rectangle.Empty;
                }
                else
                {
                    UseSameBounds = true;
                }
            }
            else
            {
                UseSameBounds = false;
            }
        }

        private void updateHitableBounds(Vector2 newPosition)
        {
            HitBoxBounds = new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            BlockingBounds = UseSameBounds ? HitBoxBounds : new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);

            UnionBounds = UseSameBounds ? HitBoxBounds : Rectangle.Union(HitBoxBounds, BlockingBounds);
        }

        protected bool canMove(Vector2 newPosition)
        {
            if (BlockingType == BlockingType.NOT_BLOCKING)
            {
                return CollisionUtils.canMove(this, new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return CollisionUtils.canMove(this, UseSameBounds ?
                    new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            base.UpdatePosition(newPosition);

            updateHitableBounds(Position);
        }
    }
}
