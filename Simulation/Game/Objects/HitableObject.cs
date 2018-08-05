using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;
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
        public BlockingType BlockingType;
        [Serialize]
        public bool IsHitable = true;

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

        protected bool canMove(WorldPosition newPosition)
        {
            Rect blockingRect = new Rect((int)(relativeBlockingBounds.X + newPosition.X), (int)(relativeBlockingBounds.Y + newPosition.Y), relativeBlockingBounds.Width, relativeBlockingBounds.Height);

            return (this is Player) ? !CollisionUtils.IsRectBlockedAccurate(this, blockingRect) : !CollisionUtils.IsRectBlockedFast(this, blockingRect);
        }

        public void ConnectToOverlappingChunks()
        {
            if (IsHitable)
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

        public override void ConnectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                // if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y) || forceLoadGridChunk)
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
                // Remove from old part
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
                    Interior interior = SimulationGame.World.InteriorManager.Get(InteriorID, false);

                    if(interior != null)
                    {
                        interior.RemoveContainedObject(this);
                    }
                }

                // Add to new part
                if (newPosition.InteriorID == Interior.Outside)
                {
                    var newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), this is DurableEntity);

                    // Add to new chunk
                    if (newChunk != null)
                    {
                        newChunk.AddContainedObject(this);
                    }
                    else
                    {
                        // Load chunk, add entity, save chunk
                        newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y));
                        newChunk.AddContainedObject(this);

                        SimulationGame.World.UnloadChunk(GeometryUtils.ConvertPointToLong((int)Position.X, (int)Position.Y));
                    }
                }
                else
                {
                    Interior interior = SimulationGame.World.InteriorManager.Get(newPosition.InteriorID, this is DurableEntity);

                    if (interior != null)
                    {
                        interior.AddContainedObject(this);
                    }
                    else
                    {
                        // Load chunk, add entity, save chunk
                        interior = SimulationGame.World.InteriorManager.Get(newPosition.InteriorID);
                        interior.AddContainedObject(this);

                        SimulationGame.World.InteriorManager.UnloadChunk(newPosition.InteriorID);
                    }
                }
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
