using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.MetaData
{
    public abstract class MetaDataType
    {
        public int ID;
        public string Name = null;

        public int YPositionDepthOffset = 0;
    }
}
