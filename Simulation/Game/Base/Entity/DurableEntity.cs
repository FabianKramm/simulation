using Microsoft.Xna.Framework;
using Newtonsoft.Json;
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

        public DurableEntity(Vector2 position, Rectangle relativeHitBoxBounds, int preloadedSurroundingWorldGridChunkRadius = 1) :
            base(position, relativeHitBoxBounds)
        {
            this.preloadedSurroundingWorldGridChunkRadius = preloadedSurroundingWorldGridChunkRadius;

            preloadWorldGridChunks();
        }

        private void preloadWorldGridChunks()
        {
            Point chunkPosition = GeometryUtils.getChunkPosition((int)position.X, (int)position.Y, World.World.WorldChunkPixelSize.X, World.World.WorldChunkPixelSize.Y);
            preloadedWorldGridChunkBounds = new Rectangle(chunkPosition.X - preloadedSurroundingWorldGridChunkRadius, chunkPosition.Y - preloadedSurroundingWorldGridChunkRadius, preloadedSurroundingWorldGridChunkRadius * 2, preloadedSurroundingWorldGridChunkRadius * 2);

            for (int i = -preloadedSurroundingWorldGridChunkRadius; i < preloadedSurroundingWorldGridChunkRadius; i++)
                for (int j = -preloadedSurroundingWorldGridChunkRadius; j < preloadedSurroundingWorldGridChunkRadius; j++)
                {
                    SimulationGame.world.getWorldGridChunk(i + chunkPosition.X, j + chunkPosition.Y);
                }
        }

        public override void updatePosition(Vector2 newPosition)
        {
            base.updatePosition(newPosition);

            preloadWorldGridChunks();
        }
    }
}
