using Microsoft.Xna.Framework;
using Simulation.Game.MetaData;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Scripts.Base;
using Simulation.Util.Geometry;

namespace Simulation.Game.Objects
{
    public class AmbientObject: GameObject
    {
        [Serialize]
        public int AmbientObjectType;

        // Json
        protected AmbientObject(): base() { }

        public AmbientObject(WorldPosition worldPosition): base(worldPosition) { }

        public AmbientObjectType GetObjectType()
        {
            return MetaData.AmbientObjectType.lookup[AmbientObjectType];
        }

        public override void Init()
        {
            base.Init();

            var ambientObjectType = GetObjectType();

            if (ambientObjectType.CustomControllerScript != null)
                CustomController = (GameObjectController)SerializationUtils.CreateInstance(ambientObjectType.CustomControllerScript);

            if (ambientObjectType.CustomRendererScript != null)
                CustomRenderer = (GameObjectRenderer)SerializationUtils.CreateInstance(ambientObjectType.CustomRendererScript);

            CustomController?.Init(this);
            CustomRenderer?.Init(this);
        }

        public override void ConnectToWorld()
        {
            if (InteriorID == Interior.Outside)
            {
                // Add as contained object to main chunk
                Point positionChunk = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                // if (SimulationGame.World.isWorldGridChunkLoaded(positionChunk.X, positionChunk.Y) || forceLoadGridChunk)
                // We load it here 
                SimulationGame.World.GetFromChunkPoint(positionChunk.X, positionChunk.Y).AddAmbientObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(InteriorID).AddAmbientObject(this);
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
                    chunk.RemoveAmbientObject(this);
            }
            else
            {
                SimulationGame.World.InteriorManager.Get(InteriorID).RemoveAmbientObject(this);
            }
        }
    }
}
