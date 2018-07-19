using Microsoft.Xna.Framework;
using Simulation.Game.Effects;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation.Game.World
{
    public abstract class WorldPart
    {
        public bool Connected = false;

        protected BlockType[,] blockingGrid;

        // These objects stay on this chunk and are drawn
        private List<HitableObject> containedObjects;
        public IList<HitableObject> ContainedObjects
        {
            get { return containedObjects == null ? null : containedObjects.AsReadOnly(); }
        }

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        public Dictionary<string, Effect> ContainedEffects;

        // These objects link to the interiors
        public volatile Dictionary<ulong, WorldLink> WorldLinks;

        protected WorldPart() { }

        public BlockType GetBlockType(int blockX, int blockY)
        {
            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            return blockingGrid[projectedPosition.X, projectedPosition.Y];
        }

        public void SetBlockType(int blockX, int blockY, BlockType blockType)
        {
            if(Connected)
            {
                throw new Exception("Cannot set block type, when already connected to world!");
            }

            var projectedPosition = GeometryUtils.GetPositionWithinChunk(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);

            blockingGrid[projectedPosition.X, projectedPosition.Y] = blockType;
        }

        public void AddEffect(Effect effect)
        {
            if (ContainedEffects == null)
                ContainedEffects = new Dictionary<string, Effect>();

            ContainedEffects[effect.ID] = effect;
        }

        public void RemoveEffect(Effect effect)
        {
            if (ContainedEffects != null)
            {
                ContainedEffects.Remove(effect.ID);

                if (ContainedEffects.Count == 0)
                {
                    ContainedEffects = null;
                }
            }
        }

        public virtual void AddWorldLink(WorldLink worldLink)
        {
            if(Connected)
            {
                throw new Exception("Cannot add world link, when already connected to world");
            }

            if (WorldLinks == null)
                WorldLinks = new Dictionary<ulong, WorldLink>();

            WorldLinks[GeometryUtils.ConvertPointToLong(worldLink.FromBlock.X, worldLink.FromBlock.Y)] = worldLink;
        }

        public virtual void RemoveWorldLink(WorldLink worldLink)
        {
            if (Connected)
            {
                throw new Exception("Cannot remove world link, when already connected to world");
            }

            if (WorldLinks != null)
            {
                WorldLinks.Remove(GeometryUtils.ConvertPointToLong(worldLink.FromBlock.X, worldLink.FromBlock.Y));

                if (WorldLinks.Count == 0)
                {
                    WorldLinks = null;
                }
            }
        }

        public virtual void AddContainedObject(HitableObject containedObject)
        {
            if (containedObjects == null)
                containedObjects = new List<HitableObject>();

            if(containedObjects.Contains(containedObject) == false)
                containedObjects.Add(containedObject);
        }

        public virtual void RemoveContainedObject(HitableObject containedObject)
        {
            if (containedObjects != null)
            {
                containedObjects.Remove(containedObject);

                if (containedObjects.Count == 0)
                {
                    containedObjects = null;
                }
            }
        }

        public void AddAmbientObject(AmbientObject ambientObject)
        {
            if (AmbientObjects == null)
                AmbientObjects = new List<AmbientObject>();

            if (AmbientObjects.Contains(ambientObject) == false)
                AmbientObjects.Add(ambientObject);
        }

        public void RemoveAmbientObject(AmbientObject ambientObject)
        {
            if (AmbientObjects != null)
            {
                AmbientObjects.Remove(ambientObject);

                if (AmbientObjects.Count == 0)
                {
                    AmbientObjects = null;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update Effects
            for (int index = 0; ContainedEffects != null && index < ContainedEffects.Count; index++) // Avoid collection changed problem 
            {
                var effect = ContainedEffects.ElementAt(index).Value;
                effect.Update(gameTime);

                if (effect.IsFinished)
                {
                    ContainedEffects.Remove(effect.ID);
                    index--;
                }
            }

            // Update Contained Objects
            for (int i = 0; ContainedObjects != null && i < ContainedObjects.Count; i++) // Avoid collection changed problem with updatePosition and disconnectWorld
            {
                var containedObject = ContainedObjects[i];

                if (containedObject is MovingEntity)
                    ((MovingEntity)containedObject).Update(gameTime);
            }
        }
    }
}
