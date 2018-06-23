using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game;
using Simulation.Game.Factories;
using Simulation.Game.World;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
            //UInt32[] loadedChunk = new UInt32[WalkableGrid.WalkableGridChunkCount];

            //WalkableGrid.changePositionInChunk(loadedChunk, 1, 4, true);

            //var arrayPosition = GeometryUtils.getIndexFromPoint(1, 3, WalkableGrid.WalkableGridChunkSize.X, WalkableGrid.WalkableGridChunkSize.Y);

            //Console.WriteLine(WalkableGrid.getBit(loadedChunk[arrayPosition / 32], arrayPosition % 32));

            SimulationGame.worldGenerator = new Game.World.Generator.WorldGenerator(123);
            WorldGridChunk chunk = new WorldGridChunk(0, 0);

            chunk.addAmbientObject(StaticObjectFactory.createSmallRocks(new Vector2(0, 0)));

            WorldLoader.saveWorldGridChunk(0, 0, chunk);

            WorldGridChunk wank = WorldLoader.loadWorldGridChunk(0, 0);

            Console.WriteLine(JsonConvert.SerializeObject(wank, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }));
            //SimulationGame.worldGenerator.resetWorld();
            /*
            WalkableGrid walkableGrid = new WalkableGrid();

            var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            walkableGrid.IsPositionWalkable(0, 1);

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            */
            /*List<Task> taskList = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                taskList.Add(Task.Run(() => {
                    Console.WriteLine("Start: " + i);
                    Console.WriteLine(i + ": " + walkableGrid.IsPositionWalkable(i, 1));
                }));
            }

            Task.WaitAll(taskList.ToArray());*/

            //stopwatch.Stop();

            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

            /*var x = -7;
            var y = -7;

            var width = 4;
            var height = 4;
            
            for(var i=x;i<(x+width);i++)
            {
                for(var j=y;j<(y+height);j++)
                {
                    Console.Write(GeometryUtils.getPositionWithinChunk(i, j, width, height));
                    Console.Write(" ");
                }

                Console.Write("\n");
            }*/


            //using (var game = new SimulationGame())
            //    game.Run();

            //var a = World.getTouchedWorldBlocksCoordinates(new Rectangle(32, 32, 32, 32)).ToArray();

            //foreach (var b in a)
            //{
            //    Console.WriteLine(b);
            //}
        }
    }
#endif
}
