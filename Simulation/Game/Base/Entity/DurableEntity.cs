using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.Renderer.Entities;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Base.Entity
{
    public class DurableEntity: MovingEntity
    {
        private int preloadedSurroundingWorldGridChunkRadius;

        public Rectangle PreloadedWorldGridChunkBounds
        {
            get; private set;
        }

        public Rectangle PreloadedWorldGridChunkPixelBounds
        {
            get; private set;
        }

        // Create from JSON
        protected DurableEntity() { }

        public DurableEntity(LivingEntityType livingEntityType, Vector2 position, Rectangle relativeHitBoxBounds, int preloadedSurroundingWorldGridChunkRadius = 1) :
            base(livingEntityType, position, relativeHitBoxBounds)
        {
            this.preloadedSurroundingWorldGridChunkRadius = preloadedSurroundingWorldGridChunkRadius;

            preloadGridChunks();
        }

        private void preloadGridChunks()
        {
            Point chunkPosition = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, World.WorldGrid.WorldChunkPixelSize.X, World.WorldGrid.WorldChunkPixelSize.Y);
            PreloadedWorldGridChunkBounds = new Rectangle(chunkPosition.X - preloadedSurroundingWorldGridChunkRadius, chunkPosition.Y - preloadedSurroundingWorldGridChunkRadius, preloadedSurroundingWorldGridChunkRadius * 2 + 1, preloadedSurroundingWorldGridChunkRadius * 2 + 1);
            PreloadedWorldGridChunkPixelBounds = new Rectangle(PreloadedWorldGridChunkBounds.X * World.WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Y * World.WorldGrid.WorldChunkPixelSize.Y, PreloadedWorldGridChunkBounds.Width * World.WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Height * World.WorldGrid.WorldChunkPixelSize.Y);

            for (int i = PreloadedWorldGridChunkBounds.Left; i <= PreloadedWorldGridChunkBounds.Right - 1; i++)
                for (int j = PreloadedWorldGridChunkBounds.Top; j <= PreloadedWorldGridChunkBounds.Bottom - 1; j++)
                    SimulationGame.World.loadWorldGridChunkAsync(i, j);
        }

        public override void UpdatePosition(Vector2 newPosition)
        {
            base.UpdatePosition(newPosition);

            preloadGridChunks();
        }
    }
}
