using Microsoft.Xna.Framework;
using Simulation.Game.World;
using Simulation.Util.Collision;
using System;
using System.Diagnostics;

namespace Simulation.Game.AI
{
    public static class AIUtils
    {
        private static Random random = new Random();

        public static WorldPosition GetWalkablePositionFrom(WorldPosition realOrigin, WorldPosition realTarget, int realDistanceFromOrigin)
        {
            Debug.Assert(realOrigin.InteriorID == realTarget.InteriorID, "Problem origin != target interior");

            var direction = new Vector2(realTarget.X - realOrigin.X, realTarget.Y - realOrigin.Y);
            direction.Normalize();

            var realPosition = new WorldPosition(realOrigin.X + realDistanceFromOrigin * direction.X, realOrigin.Y + realDistanceFromOrigin * direction.Y, realOrigin.InteriorID);
            var isBlockWalkable = CollisionUtils.IsRealPositionWalkable(realPosition);
            var worldLink = SimulationGame.World.GetWorldLinkFromRealPosition(realPosition);

            if (isBlockWalkable && worldLink == null)
                return realPosition;

            return null;
        }

        public static WorldPosition GetWalkablePositionCloseTo(WorldPosition realOrigin, WorldPosition realTarget, int realDistanceFromTarget)
        {
            Debug.Assert(realOrigin.InteriorID == realTarget.InteriorID, "Problem origin != target interior");

            var direction = new Vector2(realTarget.X - realOrigin.X, realTarget.Y - realOrigin.Y);
            direction.Normalize();

            var realPosition = new WorldPosition(realTarget.X - realDistanceFromTarget * direction.X, realTarget.Y - realDistanceFromTarget * direction.Y, realOrigin.InteriorID);
            var isBlockWalkable = CollisionUtils.IsRealPositionWalkable(realPosition);
            var worldLink = SimulationGame.World.GetWorldLinkFromRealPosition(realPosition);

            if (isBlockWalkable && worldLink == null)
                return realPosition;

            return null;
        }

        public static WorldPosition GetWalkablePositionInRadius(WorldPosition realWorldPosition, int realRadius)
        {
            for(int i=0;i<20;i++)
            {
                int newX = random.Next((int)realWorldPosition.X - realRadius, (int)realWorldPosition.X + realRadius);

                int highestY = (int)Math.Floor(Math.Sqrt(realRadius * realRadius - (newX - realWorldPosition.X) * (newX - realWorldPosition.X))) + (int)realWorldPosition.Y;
                int smallestY = (int)realWorldPosition.Y - (highestY - (int)realWorldPosition.Y);

                Point randomPoint = new Point(newX, random.Next(0, 2) == 0 ? highestY : smallestY);

                var realPosition = new WorldPosition(randomPoint, realWorldPosition.InteriorID);
                var isBlockWalkable = CollisionUtils.IsRealPositionWalkable(realPosition);
                var worldLink = SimulationGame.World.GetWorldLinkFromRealPosition(realPosition);

                if (isBlockWalkable && worldLink == null)
                    return realPosition;
            }

            return null;
        }
    }
}
