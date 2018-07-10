using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.Objects
{
    public abstract class GameObject
    {
        public string InteriorID = null;

        public Vector2 Position
        {
            get; private set;
        }

        public Point BlockPosition
        {
            get; private set;
        }

        public string ID
        {
            get; private set;
        }

        public bool IsDestroyed
        {
            get; private set;
        }

        // Create from JSON
        protected GameObject() {}

        protected GameObject(Vector2 position)
        {
            Position = position;
            BlockPosition = GeometryUtils.GetChunkPosition((int)Position.X, (int)Position.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            ID = Util.Util.getUUID();
        }

        public virtual void UpdatePosition(Vector2 newPosition)
        {
            Position = new Vector2(newPosition.X, newPosition.Y);
            BlockPosition = GeometryUtils.GetChunkPosition((int)newPosition.X, (int)newPosition.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
