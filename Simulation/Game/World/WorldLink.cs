using Microsoft.Xna.Framework;

namespace Simulation.Game.World
{
    public class WorldLink
    {
        public string FromInteriorID = null;
        public Point FromBlock;
       
        public string ToInteriorID = null;
        public Point ToBlock;

        public WorldLink(Point fromBlock, string fromInteriorID, Point toBlock, string toInteriorID)
        {
            FromBlock = fromBlock;
            FromInteriorID = fromInteriorID;

            ToBlock = toBlock;
            ToInteriorID = toInteriorID;
        }
    }
}
