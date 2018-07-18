using Microsoft.Xna.Framework;
using Simulation.Game.Objects;
using Simulation.Game.Generator.Factories;
using Simulation.Game.Renderer;
using Simulation.Game.Serialization;
using Simulation.Game.World;
using Simulation.Game.Generator;
using System;
using System.Collections.Generic;
using Simulation.Util.Geometry;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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
            // Console.WriteLine("Hello World!");

            //Console.WriteLine(WorldObjectSerializer.Deserialize(WorldObjectSerializer.Serialize(AmbientObjectFactory.createTree(new Vector2(0,0)))));

            //WorldGenerator.ResetWorld();

            //using (var game = new SimulationGame())
            //  game.Run();           

            //ulong point = GeometryUtils.ConvertPointToLong(0, 0);

            //Console.WriteLine(GeometryUtils.GetPointFromLong(point));

            /* var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            for (int i = -10000000; i < 0; i++)
            {
                GeometryUtils.GetChunkPosition(i, i, 32, 32);
            }

            stopwatch.Stop();
            Console.WriteLine("Elapsed: " + stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();

            stopwatch.Start();

            for (int i = -10000000; i < 0; i++)
            {
                GeometryUtils.GetChunkPositionNew(i, i, 32, 32);
            }
            
            stopwatch.Stop();
            Console.WriteLine("Elapsed: " + stopwatch.ElapsedMilliseconds); */

            // WorldGenerator.ResetWorld();

            //UInt32[] loadedChunk = new UInt32[WalkableGrid.WalkableGridChunkCount];

            //WalkableGrid.changePositionInChunk(loadedChunk, 1, 4, true);

            //var arrayPosition = GeometryUtils.getIndexFromPoint(1, 3, WalkableGrid.WalkableGridChunkSize.X, WalkableGrid.WalkableGridChunkSize.Y);

            //Console.WriteLine(WalkableGrid.getBit(loadedChunk[arrayPosition / 32], arrayPosition % 32));

            //SimulationGame.worldGenerator.resetWorld();
            /*
            WalkableGrid walkableGrid = new WalkableGrid();

            var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            walkableGrid.IsPositionWalkable(0, 1);

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            */

            /*
            DictionaryLock dicLock = new DictionaryLock();

            List<Task> taskList = new List<Task>();
            var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            for (int i = 0; i < 10; i++)
            {
                taskList.Add(Task.Run(() => {
                    dicLock.executeGuardedSync("test", () =>
                    {
                        Thread.Sleep(100);
                        Console.WriteLine("Start: " + i);
                    });
                }));
            }
        
            Task.WaitAll(taskList.ToArray());
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);*/


            //stopwatch.Stop();

            //Console.WriteLine(stopwatch.ElapsedMilliseconds);

            /*var x = 0;
            var y = -32;

            var width = 32;
            var height = 32;
            
            for(var i=x;i<(x+width);i++)
            {
                for(var j=y;j<(y+height);j++)
                {
                    Console.Write(GeometryUtils.getIndexFromPoint(i, j, width, height));
                    Console.Write(" ");
                }

                Console.Write("\n");
            }*/

            //var a = World.getTouchedWorldBlocksCoordinates(new Rectangle(32, 32, 32, 32)).ToArray();

            //foreach (var b in a)
            //{
            //    Console.WriteLine(b);
            //}
        }
    }
#endif
}
