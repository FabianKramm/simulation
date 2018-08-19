using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects
{
    public abstract class HitableObject: GameObject
    {
        [Serialize]
        protected bool isBlocking;

        [Serialize]
        protected bool isHitable = true;

        protected Rect relativeHitBoxBounds;
        protected Rect relativeBlockingBounds;

        public Rect HitBoxBounds
        {
            get; private set;
        }

        public Rect BlockingBounds
        {
            get; private set;
        }

        public Rect UnionBounds
        {
            get; private set;
        }

        // Create from JSON
        protected HitableObject(): base() { }

        protected HitableObject(WorldPosition worldPosition): base(worldPosition){}

        public override void Init()
        {
            base.Init();

            updateHitableBounds(Position);
        }

        private void updateHitableBounds(WorldPosition newPosition)
        {
            HitBoxBounds = new Rect((int)(relativeHitBoxBounds.X + newPosition.X), (int)(relativeHitBoxBounds.Y + newPosition.Y), relativeHitBoxBounds.Width, relativeHitBoxBounds.Height);
            BlockingBounds = new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);
            UnionBounds = Rect.Union(HitBoxBounds, BlockingBounds);
        }

        public bool CanMove(WorldPosition newPosition)
        {
            Rect blockingRect = new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);

            return (this is Player) ? !CollisionUtils.IsRectBlockedAccurate(this, blockingRect) : !CollisionUtils.IsRectBlockedFast(this, blockingRect);
        }

        public void ConnectToOverlappingChunks()
        {
            if (IsHitable())
            {
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        if (chunkX == positionChunk.X && chunkY == positionChunk.Y)
                            continue;

                        var chunkPos = GeometryUtils.ConvertPointToLong(chunkX, chunkY);
                        var chunk = SimulationGame.World.Get(chunkPos, false);

                        if (chunk != null)
                        {
                            chunk.AddOverlappingObject(this);
                        }
                    }

                if (isBlocking)
                    SimulationGame.World.WalkableGrid.BlockRect(BlockingBounds);
            }
        }

        private void disconnectFromOverlappingChunks()
        {
            Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkTopLeft = GeometryUtils.GetChunkPosition(UnionBounds.Left, UnionBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            Point chunkBottomRight = GeometryUtils.GetChunkPosition(UnionBounds.Right, UnionBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            
            for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                {
                    if (chunkX == positionChunk.X && chunkY == positionChunk.Y)
                        continue;

                    var chunkPos = GeometryUtils.ConvertPointToLong(chunkX, chunkY);
                    var chunk = SimulationGame.World.Get(chunkPos, false);

                    if (chunk != null)
                    {
                        chunk.RemoveOverlappingObject(this);
                    }
                }

            if (isBlocking)
                SimulationGame.World.WalkableGrid.UnblockRect(BlockingBounds);
        }

        public override void ConnectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                // We load it here 
                SimulationGame.World.GetFromChunkPoint(positionChunk.X, positionChunk.Y).AddContainedObject(this);

                ConnectToOverlappingChunks();
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(InteriorID).AddContainedObject(this);
            }
        }

        public override void DisconnectFromWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Remove contained object from main chunk
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

        public void SetBlocking(bool blocking)
        {
            isBlocking = blocking;
        }

        public void SetHitable(bool hitable)
        {
            isHitable = hitable;
        }

        public virtual bool IsBlocking()
        {
            return isBlocking;
        }

        public virtual bool IsHitable()
        {
            return isHitable;
        }

        public override void UpdatePosition(WorldPosition newPosition)
        {
            if(this is Player && SimulationGame.IsGodMode)
            {
                base.UpdatePosition(newPosition);
                updateHitableBounds(Position);
                return;
            }
            
            if (InteriorID == Interior.Outside && newPosition.InteriorID == Interior.Outside)
            {
                Point oldWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if (oldWorldGridChunkPoint.X != newWorldGridChunkPoint.X || oldWorldGridChunkPoint.Y != newWorldGridChunkPoint.Y)
                {
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), this is DurableEntity);

                    // Remove from old chunk
                    DisconnectFromWorld();

                    // Update Position
                    base.UpdatePosition(newPosition);
                    updateHitableBounds(Position);

                    // Connect to world and load chunk if necessary
                    ConnectToWorld();

                    // Unload chunk if we just loaded it and we are not a durable entity
                    if (newChunk == null)
                    {
                        SimulationGame.World.UnloadChunk(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y));
                    }

                    return;
                }
            }
            else if (InteriorID != newPosition.InteriorID)
            {
                // Remove from old part
                DisconnectFromWorld();

                // Update Position
                base.UpdatePosition(newPosition);
                updateHitableBounds(Position);

                // Add to new part
                if (newPosition.InteriorID == Interior.Outside)
                {
                    var newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), this is DurableEntity);

                    ConnectToWorld();

                    // Unload if we just loaded
                    if (newChunk == null)
                    {
                        SimulationGame.World.UnloadChunk(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y));
                    }
                }
                else
                {
                    Interior interior = SimulationGame.World.InteriorManager.Get(newPosition.InteriorID, this is DurableEntity);

                    ConnectToWorld();

                    // Unload if we just loaded
                    if (interior == null)
                    {
                        SimulationGame.World.InteriorManager.UnloadChunk(newPosition.InteriorID);
                    }
                }

                return;
            }

            if (Position.InteriorID == Interior.Outside)
                disconnectFromOverlappingChunks();

            base.UpdatePosition(newPosition);
            updateHitableBounds(Position);

            if (Position.InteriorID == Interior.Outside)
                ConnectToOverlappingChunks();
        }
    }
}
