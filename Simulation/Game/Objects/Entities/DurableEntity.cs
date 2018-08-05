using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Diagnostics;

namespace Simulation.Game.Objects.Entities
{
    public class DurableEntity: MovingEntity
    {
        [Serialize]
        public int PreloadedSurroundingWorldGridChunkRadius = 1;

        public Rect PreloadedWorldGridChunkBounds
        {
            get; private set;
        }
        
        public Rect PreloadedWorldGridChunkPixelBounds
        {
            get; private set;
        }

        // Create from JSON
        protected DurableEntity() : base() { }

        public DurableEntity(WorldPosition worldPosition): base(worldPosition) { }

        private void preloadGridChunks()
        {
            var partsLoaded = 0;
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            Point chunkPosition = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
            PreloadedWorldGridChunkBounds = new Rect(chunkPosition.X - PreloadedSurroundingWorldGridChunkRadius, chunkPosition.Y - PreloadedSurroundingWorldGridChunkRadius, PreloadedSurroundingWorldGridChunkRadius * 2 + 1, PreloadedSurroundingWorldGridChunkRadius * 2 + 1);
            PreloadedWorldGridChunkPixelBounds = new Rect(PreloadedWorldGridChunkBounds.X * WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Y * WorldGrid.WorldChunkPixelSize.Y, PreloadedWorldGridChunkBounds.Width * WorldGrid.WorldChunkPixelSize.X, PreloadedWorldGridChunkBounds.Height * WorldGrid.WorldChunkPixelSize.Y);

            for (int i = PreloadedWorldGridChunkBounds.Left; i <= PreloadedWorldGridChunkBounds.Right; i++)
                for (int j = PreloadedWorldGridChunkBounds.Top; j <= PreloadedWorldGridChunkBounds.Bottom; j++)
                {
                    if(!SimulationGame.World.IsLoaded(GeometryUtils.ConvertPointToLong(i, j)))
                    {
                        if(SimulationGame.World.LoadAsync(GeometryUtils.ConvertPointToLong(i, j)))
                            partsLoaded++;
                    }
                }

            stopwatch.Stop();

            if (partsLoaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Preloading " + partsLoaded + " parts took " + stopwatch.ElapsedMilliseconds + "ms");
            }
        }

        public override void Init()
        {
            preloadGridChunks();

            base.Init();
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
