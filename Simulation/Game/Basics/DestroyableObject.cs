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
    public class DestroyableObject: CollidableRectangleObject
    {
        public DestroyableObject (Vector2 position, Point upperLeftPointVector, Point collisionRectSize, CollisionType collisionType = CollisionType.NO_COLLISION) : 
            base(position, upperLeftPointVector, collisionRectSize, collisionType)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
