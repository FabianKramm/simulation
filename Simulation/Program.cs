using Microsoft.Xna.Framework;
using Simulation.Game;
using System;
using System.Collections.Generic;

namespace Simulation
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //using (var game = new SimulationGame())
            //    game.Run();
            Vector2[] poly1 = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(3, 3),
                new Vector2(3, 0)
            };

            Vector2[] poly2 = new Vector2[] {
                new Vector2(1, 0),
                new Vector2(3, 3),
                new Vector2(3, 0)
            };

            Console.WriteLine(CollisionDetection.intersect(poly1, poly2));
        }
    }
#endif
}
