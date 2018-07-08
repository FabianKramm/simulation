using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Util
{
    public class TimeUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCurrentDayTick() => (int)SimulationGame.Ticks % SimulationGame.TicksPerDay;
    }
}
