using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Vector2 Position
        {
            get; private set;
        }

        public string InteriorID;

        public LivingEntity Origin
        {
            get; private set;
        }

        public bool IsFinished
        {
            get; protected set;
        }

        public Effect(Vector2 position, LivingEntity origin, string interiorID = null)
        {
            Origin = origin;
            Position = position;
            InteriorID = interiorID;

            IsFinished = false;

            ID = Util.Util.getUUID();
        }

        protected void updatePosition(Vector2 newPosition)
        {
            if (InteriorID == Interior.Outside)
            {
                // Check if we were on a worldLink
                Point oldWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point newWorldGridChunkPoint = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                if(oldWorldGridChunkPoint.X != newWorldGridChunkPoint.X || oldWorldGridChunkPoint.Y != newWorldGridChunkPoint.Y)
                {
                    // Remove from old chunk
                    if (SimulationGame.World.isWorldGridChunkLoaded(oldWorldGridChunkPoint.X, oldWorldGridChunkPoint.Y))
                        SimulationGame.World.GetWorldGridChunk(oldWorldGridChunkPoint.X, oldWorldGridChunkPoint.Y).RemoveEffect(this);
                    
                    // Add to new chunk
                    if(SimulationGame.World.isWorldGridChunkLoaded(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y))
                        SimulationGame.World.GetWorldGridChunk(newWorldGridChunkPoint.X, newWorldGridChunkPoint.Y).AddEffect(this);
                }
            }

            Position = newPosition;
        }

        public abstract void Update(GameTime gameTime);
    }
}
