using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game.World;
using Simulation.Util;

namespace Simulation.Game.Base.Entity
{
    public class DurableEntity: MovingEntity
    {
        [JsonProperty]
        private int preloadedSurroundingWorldGridChunkRadius;

        public Rectangle preloadedWorldGridChunkBounds
        {
            get; private set;
        }

        public Rectangle preloadedWorldGridChunkPixelBounds
        {
            get; private set;
        }

        public DurableEntity(Vector2 position, Rectangle relativeHitBoxBounds, int preloadedSurroundingWorldGridChunkRadius = 1) :
            base(position, relativeHitBoxBounds)
        {
            this.preloadedSurroundingWorldGridChunkRadius = preloadedSurroundingWorldGridChunkRadius;
            this.blockingType = BlockingType.BLOCKING;

            preloadGridChunks();
        }

        private void preloadGridChunks()
        {
            Point chunkPosition = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.Y);
            preloadedWorldGridChunkBounds = new Rectangle(chunkPosition.X - preloadedSurroundingWorldGridChunkRadius, chunkPosition.Y - preloadedSurroundingWorldGridChunkRadius, preloadedSurroundingWorldGridChunkRadius * 2 + 1, preloadedSurroundingWorldGridChunkRadius * 2 + 1);
            preloadedWorldGridChunkPixelBounds = new Rectangle(preloadedWorldGridChunkBounds.X * World.World.WorldChunkPixelSize.X, preloadedWorldGridChunkBounds.Y * World.World.WorldChunkPixelSize.Y, preloadedWorldGridChunkBounds.Width * World.World.WorldChunkPixelSize.X, preloadedWorldGridChunkBounds.Height * World.World.WorldChunkPixelSize.Y);

            for (int i = preloadedWorldGridChunkBounds.Left; i < preloadedWorldGridChunkBounds.Right; i++)
                for (int j = preloadedWorldGridChunkBounds.Top; j < preloadedWorldGridChunkBounds.Bottom; j++)
                    SimulationGame.world.loadWorldGridChunkAsync(i, j);
        }

        public override void updatePosition(Vector2 newPosition)
        {
            base.updatePosition(newPosition);

            preloadGridChunks();
        }
    }
}
