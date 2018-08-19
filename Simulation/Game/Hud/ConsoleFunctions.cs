using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Hud
{
    public class ConsoleFunctions
    {
        public static void SetNight()
        {
            SimulationGame.Ticks = 0;
        }

        public static void SetDay()
        {
            SimulationGame.Ticks = SimulationGame.TicksPerHour * 12;
        }
    }
}
