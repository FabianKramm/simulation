using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.World;

namespace Simulation.Game.Generator.InteriorGeneration
{
    public class InteriorGenerator
    {
        public static Interior CreateInterior(out WorldLink entranceLink, Point fromBlock, string fromInteriorID = null)
        {
            int dimX = SimulationGame.WorldGenerator.random.Next(10, 40);
            int dimY = SimulationGame.WorldGenerator.random.Next(10, 40);

            int linkX = SimulationGame.WorldGenerator.random.Next(1, dimX);
            int linkY = SimulationGame.WorldGenerator.random.Next(1, dimY);

            Interior retInterior = new Interior(new Point(dimX, dimY));

            for(int i=0;i<dimX;i++)
            {
                for(int j=0;j<dimY;j++)
                {
                    retInterior.SetBlockType(i, j, 1);
                }
            }

            entranceLink = new WorldLink(fromBlock, fromInteriorID, new Point(linkX, linkY), retInterior.ID);
            retInterior.AddWorldLink(new WorldLink(new Point(linkX, linkY), retInterior.ID, fromBlock, fromInteriorID));

            return retInterior;
        }
    }
}
