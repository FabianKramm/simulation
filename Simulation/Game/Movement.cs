using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game
{
    public enum WalkingDirection
    {
        Idle = 0,
        Left,
        Right,
        Up,
        Down
    };

    public class Movement
    {
        public static WalkingDirection getWalkingDirectionFromVector(Vector2 direction)
        {
            if(direction == Vector2.Zero)
            {
                return WalkingDirection.Idle;
            }

            if(direction.X > 0)
            {
                return WalkingDirection.Right;
            }

            if (direction.X < 0)
            {
                return WalkingDirection.Left;
            }

            if (direction.Y > 0)
            {
                return WalkingDirection.Up;
            }

            if (direction.Y < 0)
            {
                return WalkingDirection.Down;
            }

            return WalkingDirection.Idle;
        }
    }
}
