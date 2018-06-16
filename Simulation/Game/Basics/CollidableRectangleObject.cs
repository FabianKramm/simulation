using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Hud;
using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    public class CollidableRectangleObject: DrawableObject
    {
        private Point upperLeftPointVector;
        private Point collisionRectSize;
        public Rectangle collisionBounds;
        public CollisionType collisionType;

        public CollidableRectangleObject(Vector2 position, Point upperLeftPointVector, Point collisionRectSize, CollisionType collisionType = CollisionType.NO_COLLISION): base(position)
        {
            this.upperLeftPointVector = upperLeftPointVector;
            this.collisionRectSize = collisionRectSize;
            this.collisionType = collisionType;

            onPositionChange();
        }

        public bool canMove(Vector2 newPosition)
        {
            Rectangle newBounds = new Rectangle((int)newPosition.X + upperLeftPointVector.X, (int)newPosition.Y + upperLeftPointVector.Y, collisionRectSize.X, collisionRectSize.Y);
            List<Block> blocks = SimulationGame.world.getTouchedWorldBlocks(ref newBounds);

            foreach(Block block in blocks)
            {
                if (block.collisionType == BlockCollisionType.UNPASSABLE)
                    return false;

                foreach(CollidableRectangleObject collidableObject in block.collidableObjects)
                {
                    if(collidableObject.collisionType == CollisionType.SOLID_OBJECT && collidableObject.collisionBounds.Intersects(newBounds))
                    {
                        return false;
                    }
                }
            }

            if(SimulationGame.isDebug)
            {
                string DebugString = "";

                foreach (Block block in blocks)
                {
                    DebugString += " (" + block.position.X + "," + block.position.Y;

                    DebugString += ",CO: " + block.collidableObjects.Count;

                    DebugString += ")";
                }

                GameConsole.WriteLine("CanMoveCollision", DebugString);
            }

            return true;
        }
        
        protected override void onPositionChange()
        {
            collisionBounds = new Rectangle((int)position.X + upperLeftPointVector.X, (int)position.Y + upperLeftPointVector.Y, collisionRectSize.X, collisionRectSize.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(SimulationGame.isDebug)
            {
                SimulationGame.primitiveDrawer.Rectangle(new Rectangle((int)position.X + upperLeftPointVector.X, (int)position.Y + upperLeftPointVector.Y, collisionRectSize.X, collisionRectSize.Y), Color.Red);
            }
        }
    }
}
