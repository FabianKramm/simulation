using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Basics
{
    class CollidableRectangleObject: DrawableObject
    {
        private Point upperLeftPointVector;
        private Point collisionRectSize;

        public CollidableRectangleObject(Point position, Point upperLeftPointVector, Point collisionRectSize): base(position)
        {
            this.upperLeftPointVector = upperLeftPointVector;
            this.collisionRectSize = collisionRectSize;
        }

        public bool isCollision(Point newPosition)
        {
            List<Block> blocks = SimulationGame.world.getTouchedWorldBlocks(new Rectangle(newPosition.X + upperLeftPointVector.X, newPosition.Y + upperLeftPointVector.Y, collisionRectSize.X, collisionRectSize.Y));

            foreach(Block block in blocks)
                if(block.collisionType == CollisionType.UNPASSABLE)
                    return true;

            if(SimulationGame.isDebug)
            {
                string DebugString = "";

                foreach (Block block in blocks)
                    DebugString += " (" + block.position.X + "," + block.position.Y + ")";

                SimulationGame.StringToDraw = DebugString;
            }

            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(SimulationGame.isDebug)
            {
                SimulationGame.primitiveDrawer.Rectangle(new Rectangle(position.X + upperLeftPointVector.X, position.Y + upperLeftPointVector.Y, collisionRectSize.X, collisionRectSize.Y), Color.Red);
            }
        }
    }
}
