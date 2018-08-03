using Microsoft.Xna.Framework;
using Simulation.Game.Effects;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulation.Game.World
{
    public abstract class WorldPart
    {
        [Serialize]
        public bool IsPersistent
        {
            get; protected set;
        }

        [Serialize]
        public Point Dimensions
        {
            get; protected set;
        }

        [Serialize]
        protected int[,] blockingGrid;
        
        [Serialize] // These objects link to the interiors
        public volatile Dictionary<ulong, WorldLink> WorldLinks;

        public bool Connected = false;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> ContainedObjects;

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        public Dictionary<string, Effect> ContainedEffects;

        protected WorldPart() { }

        public abstract int GetBlockType(int blockX, int blockY);
        public abstract void SetBlockType(int blockX, int blockY, int blockType);

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
            if (ContainedObjects == null)
                ContainedObjects = new List<HitableObject>();

            if(ContainedObjects.Contains(containedObject) == false)
                ContainedObjects.Add(containedObject);
        }

        public virtual void RemoveContainedObject(HitableObject containedObject)
        {
            if (ContainedObjects != null)
            {
                ContainedObjects.Remove(containedObject);

                if (ContainedObjects.Count == 0)
                {
                    ContainedObjects = null;
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

            // Update Ambient Objects
            for (int i = 0; AmbientObjects != null && i < AmbientObjects.Count; i++) // Avoid collection changed problem with updatePosition and disconnectWorld
                AmbientObjects[i].Update(gameTime);

            // Update Contained Objects
            for (int i = 0; ContainedObjects != null && i < ContainedObjects.Count; i++) // Avoid collection changed problem with updatePosition and disconnectWorld
                ContainedObjects[i].Update(gameTime);
        }
    }
}
