using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects
{
    public abstract class HitableObject: GameObject
    {
        protected Rect relativeHitBoxBounds;
        protected Rect relativeBlockingBounds;

        private bool UseSameBounds;

        public BlockingType BlockingType
        {
            get; private set;
        }

        public Rect HitBoxBounds;
        public Rect BlockingBounds;

        public Rect UnionBounds;

        // Create from JSON
        protected HitableObject() { }

        public HitableObject(Vector2 position, Rect relativeHitBoxBounds, BlockingType blockingType = BlockingType.NOT_BLOCKING, Rect? relativeBlockingBounds = null)
            :base(position)
        {
            this.relativeHitBoxBounds = relativeHitBoxBounds;

            SetBlockingType(blockingType);
            updateHitableBounds(position);
        }

        public void SetBlockingType(BlockingType blockingType, Rect? relativeBlockingBounds = null)
        {
            this.BlockingType = blockingType;

            if (blockingType == BlockingType.BLOCKING)
            {
                if (relativeBlockingBounds != null)
                {
                    this.relativeBlockingBounds = relativeBlockingBounds ?? Rect.Empty;
                }
                else
                {
                    UseSameBounds = true;
                }
            }
            else
            {
                UseSameBounds = true;
            }
        }

        private void updateHitableBounds(Vector2 newPosition)
        {
            HitBoxBounds = new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            BlockingBounds = UseSameBounds ? HitBoxBounds : new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);

            UnionBounds = UseSameBounds ? HitBoxBounds : Rect.Union(HitBoxBounds, BlockingBounds);
        }

        protected bool canMove(Vector2 newPosition)
        {
            if (BlockingType == BlockingType.NOT_BLOCKING)
            {
                return CollisionUtils.canMove(this, new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return CollisionUtils.canMove(this, UseSameBounds ?
                    new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            base.UpdatePosition(newPosition);

            updateHitableBounds(Position);
        }
    }
}
