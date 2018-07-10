using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Objects;
using Simulation.Game.Renderer;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using static Simulation.Game.Renderer.BlockRenderer;

namespace Simulation.Game.World
{
    public enum BlockingType
    {
        NOT_BLOCKING = 0,
        BLOCKING
    }

    public enum HitBoxType
    {
        NO_HITBOX = 0,
        STATIC_OBJECT,
        MOVING_OBJECT,
        LIVING_ENTITY
    }

    public class Block
    {
        public BlockType blockType;

        public Rect blockBounds
        {
            get; private set;
        }

        public BlockingType blockingType
        {
            get; private set;
        }

        // Objects that are hitable and can block the path
        public List<HitableObject> hitableObjects
        {
            get; private set;
        }

        // Not yet defined
        // public List<HitBoxRectangle> interactiveObjects;

        // Objects that should be drawn on this tile
        public List<GameObject> ambientObjects
        {
            get; private set;
        }

        public Block(Point position, BlockType blockType = BlockType.GRASS_01, BlockingType blockingType = BlockingType.NOT_BLOCKING)
        {
            this.blockType = blockType;
            this.blockingType = blockingType;

            blockBounds = new Rect(position.X * WorldGrid.BlockSize.X, position.Y * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public void addHitableObject(HitableObject hitableObject)
        {
            if (hitableObjects == null)
                hitableObjects = new List<HitableObject>();

            hitableObjects.Add(hitableObject);
        }

        public void removeHitableObject(HitableObject hitableObject)
        {
            if (hitableObjects != null)
            {
                hitableObjects.Remove(hitableObject);

                if(hitableObjects.Count == 0)
                {
                    hitableObjects = null;
                }
            }
        }

        public void addAmbientObject(GameObject ambientObject)
        {
            if (ambientObjects == null)
                ambientObjects = new List<GameObject>();

            ambientObjects.Add(ambientObject);
        }
    }
}
