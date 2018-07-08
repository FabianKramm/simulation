﻿using Microsoft.Xna.Framework;
using Simulation.Game.Renderer;
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

        public Rectangle unionBounds;

        // Create from JSON
        protected HitableObject() { }

        public HitableObject(Vector2 position, Rectangle relativeHitBoxBounds, BlockingType blockingType = BlockingType.NOT_BLOCKING, Rectangle? relativeBlockingBounds = null)
            : base(position)
        {
            this.relativeHitBoxBounds = relativeHitBoxBounds;

            setBlockingType(blockingType);
            updateHitableBounds(position);
        }

        public void setBlockingType(BlockingType blockingType, Rectangle? relativeBlockingBounds = null)
        {
            this.blockingType = blockingType;

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
            else
            {
                useSameBounds = false;
            }
        }

        private void updateHitableBounds(Vector2 newPosition)
        {
            hitBoxBounds = new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            blockingBounds = useSameBounds ? hitBoxBounds : new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);

            unionBounds = useSameBounds ? hitBoxBounds : Rectangle.Union(hitBoxBounds, blockingBounds);
        }
        
        public bool canMove(Vector2 newPosition)
        {
            if(blockingType == BlockingType.NOT_BLOCKING)
            {
                return SimulationGame.world.canMove(this, new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return SimulationGame.world.canMove(this, useSameBounds ?
                    new Rectangle((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rectangle((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            base.UpdatePosition(newPosition);

            updateHitableBounds(position);
        }
    }
}
