using Microsoft.Xna.Framework;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game.Generator
{
    public class GeneratorUtils
    {
        public static Point GetRandomPointInCircle(Random random, Circle findCircle)
        {
            int newX = random.Next(findCircle.CenterX - findCircle.Radius, findCircle.CenterX + findCircle.Radius);

            int highestY = (int)Math.Floor(Math.Sqrt(findCircle.Radius * findCircle.Radius - (newX - findCircle.CenterX) * (newX - findCircle.CenterX))) + findCircle.CenterY;
            int smallestY = findCircle.CenterY - (highestY - findCircle.CenterY);

            return new Point(newX, random.Next(smallestY, highestY + 1));
        }
    }
}
