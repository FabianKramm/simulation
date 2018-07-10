using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
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
                return CollisionUtils.IsRectBlocked(this, new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return CollisionUtils.IsRectBlocked(this, UseSameBounds ?
                    new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        public void ConnectToWorld(bool forceLoadGridChunk = false)
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y) || forceLoadGridChunk)
                    SimulationGame.World.GetWorldGridChunk(positionChunk.X, positionChunk.Y).AddContainedObject(this);

                // Add as interactive object for adjoined chunks
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.GetWorldGridChunk(chunkX, chunkY).AddOverlappingObject(this);
                        }
                    }

                SimulationGame.World.walkableGrid.addInteractiveObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.GetInterior(InteriorID).AddContainedObject(this);
            }
        }

        public void DisconnectFromWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y))
                    SimulationGame.World.GetWorldGridChunk(positionChunk.X, positionChunk.Y).RemoveContainedObject(this);

                Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                        if (SimulationGame.World.isWorldGridChunkLoaded(chunkX, chunkY))
                        {
                            SimulationGame.World.GetWorldGridChunk(chunkX, chunkY).RemoveOverlappingObject(this);
                        }
                    }

                SimulationGame.World.walkableGrid.removeInteractiveObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.GetInterior(InteriorID).RemoveContainedObject(this);
            }
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            base.UpdatePosition(newPosition);

            updateHitableBounds(Position);
        }
    }
}
