﻿using Microsoft.Xna.Framework;
using Simulation.Game.Base;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public class Interior
    {
        public static string Outside = null;

        public string ID;

        private BlockType[,] blockingGrid;
        public Point Dimensions;

        // These objects stay on this chunk and are drawn
        public List<HitableObject> ContainedObjects;

        // These objects are not important for the world and are just displayed here
        public List<AmbientObject> AmbientObjects;

        // These objects are used to connect this interior to the world
        public Dictionary<Point, WorldLink> WorldLinks;

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            Dimensions = dimensions;

            blockingGrid = new BlockType[dimensions.X, dimensions.Y];
        }

        public BlockType GetBlockType(int blockX, int blockY)
        {
            if (blockX < 0 || blockX >= Dimensions.X)
                return BlockType.NONE;
            if (blockY < 0 || blockY >= Dimensions.Y)
                return BlockType.NONE;

            return blockingGrid[blockX, blockY];
        }

        public void AddWorldLink(WorldLink worldLink)
        {
            if (WorldLinks == null)
                WorldLinks = new Dictionary<Point, WorldLink>();

            if (WorldLinks.ContainsKey(worldLink.FromBlock) == false)
                WorldLinks[worldLink.FromBlock] = worldLink;
        }

        public void RemoveWorldLink(WorldLink worldLink)
        {
            if (WorldLinks != null)
            {
                WorldLinks.Remove(worldLink.FromBlock);

                if (WorldLinks.Count == 0)
                {
                    WorldLinks = null;
                }
            }
        }

        public void AddContainedObject(HitableObject containedObject)
        {
            if (ContainedObjects == null)
                ContainedObjects = new List<HitableObject>();

            if (ContainedObjects.Contains(containedObject) == false)
                ContainedObjects.Add(containedObject);
        }

        public void RemoveContainedObject(HitableObject interactiveObject)
        {
            if (ContainedObjects != null)
            {
                ContainedObjects.Remove(interactiveObject);

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
    }
}
