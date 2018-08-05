using Microsoft.Xna.Framework;

namespace Simulation.Game.World
{
    public class WorldLink
    {
        public string FromInteriorID = null;
        public Point FromBlock;
       
        public string ToInteriorID = null;
        public Point ToBlock;

        // Need for json serialize & unserialize
        public WorldLink() {}

        public WorldLink(Point fromBlock, string fromInteriorID, Point toBlock, string toInteriorID)
        {
            FromBlock = fromBlock;
            FromInteriorID = fromInteriorID;

            ToBlock = toBlock;
            ToInteriorID = toInteriorID;
        }

        public WorldLink(WorldPosition fromBlockPosition, WorldPosition toBlockPosition)
        {
            FromBlock = fromBlockPosition.ToPoint();
            FromInteriorID = fromBlockPosition.InteriorID;

            ToBlock = toBlockPosition.ToPoint();
            ToInteriorID = toBlockPosition.InteriorID;
        }

        public WorldLink Clone()
        {
            return new WorldLink(ToBlockWorldPositionFrom(), ToBlockWorldPositionTo());
        }

        public WorldLink SwapFromTo()
        {
            return new WorldLink(ToBlockWorldPositionTo(), ToBlockWorldPositionFrom());
        }

        public WorldPosition ToRealWorldPositionTo()
        {
            return new WorldPosition(ToBlock.X * WorldGrid.BlockSize.X, ToBlock.Y * WorldGrid.BlockSize.Y, ToInteriorID);
        }

        public WorldPosition ToBlockWorldPositionTo()
        {
            return new WorldPosition(ToBlock.X, ToBlock.Y, ToInteriorID);
        }

        public WorldPosition ToRealWorldPositionFrom()
        {
            return new WorldPosition(FromBlock.X * WorldGrid.BlockSize.X, FromBlock.Y * WorldGrid.BlockSize.Y, FromInteriorID);
        }

        public WorldPosition ToBlockWorldPositionFrom()
        {
            return new WorldPosition(FromBlock.X, FromBlock.Y, FromInteriorID);
        }
    }
}
