using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Renderer.Effects;
using Simulation.Game.World;
using Simulation.Util.Geometry;

namespace Simulation.Game.Effects
{
    abstract public class Effect
    {
        public EffectRendererInformation effectRendererInformation;

        public string ID
        {
            get; private set;
        }

        public WorldPosition Position
        {
            get; private set;
        }

        public string InteriorID
        {
            get => Position.InteriorID;
        }

        public LivingEntity Origin
        {
            get; private set;
        }

        public bool IsFinished
        {
            get; protected set;
        }

        public Effect(WorldPosition position, LivingEntity origin)
        {
            Origin = origin;
            Position = position;

            IsFinished = false;

            ID = Util.Util.getUUID();
        }

        protected void updatePosition(WorldPosition newPosition)
        {
            if (InteriorID == Interior.Outside)
            {
                Point oldWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if(oldWorldGridChunkPoint.X != newWorldGridChunkPoint.X || oldWorldGridChunkPoint.Y != newWorldGridChunkPoint.Y)
                {
                    var oldChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(oldWorldGridChunkPoint.X, oldWorldGridChunkPoint.Y), false);
                    var newChunk = SimulationGame.World.Get(GeometryUtils.ConvertPointToLong(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y), false);

                    // Remove from old chunk
                    if (oldChunk != null)
                        oldChunk.RemoveEffect(this);
                    
                    // Add to new chunk
                    if(newChunk != null)
                        newChunk.AddEffect(this);
                }
            }

            Position = newPosition;
        }

        public abstract void Update(GameTime gameTime);
    }
}
