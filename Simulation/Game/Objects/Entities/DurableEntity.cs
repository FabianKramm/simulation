using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects.Entities
{
    public class DurableEntity: MovingEntity
    {
        private int preloadedSurroundingWorldGridChunkRadius;

        public Rect PreloadedWorldGridChunkBounds
        {
            get; private set;
        }

        public Rect PreloadedWorldGridChunkPixelBounds
        {
            get; private set;
        }

        // Create from JSON
        protected DurableEntity() { }

        public DurableEntity(LivingEntityType livingEntityType, WorldPosition position, FractionType fraction, int preloadedSurroundingWorldGridChunkRadius = 1) :
            base(livingEntityType, position, fraction)
        {
            this.preloadedSurroundingWorldGridChunkRadius = preloadedSurroundingWorldGridChunkRadius;

            preloadGridChunks();
        }

        public DurableEntity(LivingEntityType livingEntityType, WorldPosition position, Rect relativeHitBoxBounds, FractionType fraction, int preloadedSurroundingWorldGridChunkRadius = 1) :
            base(livingEntityType, position, relativeHitBoxBounds, fraction)
        {
            this.preloadedSurroundingWorldGridChunkRadius = preloadedSurroundingWorldGridChunkRadius;

            preloadGridChunks();
        }

        private void preloadGridChunks()
        {
            Point chunkPosition = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            PreloadedWorldGridChunkBounds = new Rect(chunkPosition.X - preloadedSurroundingWorldGridChunkRadius, chunkPosition.Y - preloadedSurroundingWorldGridChunkRadius, preloadedSurroundingWorldGridChunkRadius * 2 + 1, preloadedSurroundingWorldGridChunkRadius * 2 + 1);
            PreloadedWorldGridChunkPixelBounds = new Rect(PreloadedWorldGridChunkBounds.X * WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Y * WorldGrid.WorldChunkPixelSize.Y, PreloadedWorldGridChunkBounds.Width * WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Height * WorldGrid.WorldChunkPixelSize.Y);

            for (int i = PreloadedWorldGridChunkBounds.Left; i <= PreloadedWorldGridChunkBounds.Right; i++)
                for (int j = PreloadedWorldGridChunkBounds.Top; j <= PreloadedWorldGridChunkBounds.Bottom; j++)
                {
                    if(!SimulationGame.World.IsLoaded(GeometryUtils.ConvertPointToLong(i, j)))
                        SimulationGame.World.LoadAsync(GeometryUtils.ConvertPointToLong(i, j));
                }
                    
        }

        protected override void UpdatePosition(WorldPosition newPosition)
        {
            base.UpdatePosition(newPosition);

            if(InteriorID == Interior.Outside)
                preloadGridChunks();
        }

        public override void Update(GameTime gameTime)
        {
            if (InteriorID == Interior.Outside)
                preloadGridChunks();

            base.Update(gameTime);
        }
    }
}
