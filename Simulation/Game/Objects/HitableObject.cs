using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
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

        public HitableObject(WorldPosition position, Rect relativeHitBoxBounds, BlockingType blockingType = BlockingType.NOT_BLOCKING, Rect? relativeBlockingBounds = null)
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

        private void updateHitableBounds(WorldPosition newPosition)
        {
            HitBoxBounds = new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            BlockingBounds = UseSameBounds ? HitBoxBounds : new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);
            UnionBounds = UseSameBounds ? HitBoxBounds : Rect.Union(HitBoxBounds, BlockingBounds);
        }

        protected bool canMove(WorldPosition newPosition)
        {
            if (BlockingType == BlockingType.NOT_BLOCKING)
            {
                return !CollisionUtils.IsRectBlocked(this, new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height));
            }
            else
            {
                return !CollisionUtils.IsRectBlocked(this, UseSameBounds ?
                    new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height) :
                    new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height));
            }
        }

        private void connectToOverlappingChunks()
        {
            Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                    var chunkPos = GeometryUtils.ConvertPointToLong(chunkX, chunkY);
                    var chunk = SimulationGame.World.Get(chunkPos, false);

                    if (chunk != null)
                    {
                        chunk.AddOverlappingObject(this);
                    }
                }

            if (BlockingType == BlockingType.BLOCKING)
                SimulationGame.World.WalkableGrid.BlockRect(BlockingBounds);
        }

        private void disconnectFromOverlappingChunks()
        {
            Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if (chunkX == positionChunk.X && chunkY == positionChunk.Y) continue;

                    var chunkPos = GeometryUtils.ConvertPointToLong(chunkX, chunkY);
                    var chunk = SimulationGame.World.Get(chunkPos, false);

                    if (chunk != null)
                    {
                        chunk.RemoveOverlappingObject(this);
                    }
                }

            if (BlockingType == BlockingType.BLOCKING)
                SimulationGame.World.WalkableGrid.UnblockRect(BlockingBounds);
        }

        public void ConnectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                // if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y) || forceLoadGridChunk)
                // We load it here 
                SimulationGame.World.GetFromChunkPoint(positionChunk.X, positionChunk.Y).AddContainedObject(this);

                connectToOverlappingChunks();
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(InteriorID).AddContainedObject(this);
            }
        }

        public void DisconnectFromWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                var chunkPos = GeometryUtils.ConvertPointToLong(positionChunk.X, positionChunk.Y);
                var chunk = SimulationGame.World.Get(chunkPos, false);

                if (chunk != null)
                    chunk.RemoveContainedObject(this);

                disconnectFromOverlappingChunks();
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(InteriorID).RemoveContainedObject(this);
            }
        }

        protected override void UpdatePosition(WorldPosition newPosition)
        {
            if(InteriorID == Interior.Outside && newPosition.InteriorID == Interior.Outside)
            {
                Point oldWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (oldWorldGridChunkPoint.X != newWorldGridChunkPoint.X || oldWorldGridChunkPoint.Y != newWorldGridChunkPoint.Y)
                {
                    var oldChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(oldWorldGridChunkPoint.X, oldWorldGridChunkPoint.Y), false);
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), false);

                    // Remove from old chunk
                    if (InteriorID == Interior.Outside && oldChunk != null)
                        oldChunk.RemoveContainedObject(this);

                    // TODO: What happens if not loaded??
                    // Add to new chunk
                    if (newPosition.InteriorID == Interior.Outside && newChunk != null)
                        newChunk.AddContainedObject(this);
                }
            } 
            else if(InteriorID != newPosition.InteriorID)
            {
                if(InteriorID == Interior.Outside)
                {
                    Point oldWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    var oldChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(oldWorldGridChunkPoint.X, oldWorldGridChunkPoint.Y), false);

                    // Remove from old chunk
                    if (InteriorID == Interior.Outside && oldChunk != null)
                        oldChunk.RemoveContainedObject(this);
                }
                else
                {
                    SimulationGame.World.InteriorManager.Get(InteriorID).RemoveContainedObject(this);
                }

                if (newPosition.InteriorID == Interior.Outside)
                {
                    Point newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), false);

                    // TODO: What happens if not loaded??
                    // Add to new chunk
                    if (newPosition.InteriorID == Interior.Outside && newChunk != null)
                        newChunk.AddContainedObject(this);
                }
                else
                {
                    SimulationGame.World.InteriorManager.Get(newPosition.InteriorID).AddContainedObject(this);
                }
            }

            if (Position.InteriorID == Interior.Outside)
                disconnectFromOverlappingChunks();

            base.UpdatePosition(newPosition);
            updateHitableBounds(Position);

            if (Position.InteriorID == Interior.Outside)
                connectToOverlappingChunks();
        }
    }
}
