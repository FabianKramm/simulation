using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Simulation.Game;
using Simulation.Game.Factories;
using Simulation.Game.World;
using Simulation.Game.World.Generator;
using Simulation.PathFinding;
using Simulation.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
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
            BaseGrid searchGrid = new StaticGrid(64, 32);

            searchGrid.SetWalkableAt(10, 20, true);

            GridPos startPos = new GridPos(10, 10);
            GridPos endPos = new GridPos(20, 11);
            JumpPointParam jpParam = new JumpPointParam(searchGrid, startPos, endPos, EndNodeUnWalkableTreatment.DISALLOW, DiagonalMovement.OnlyWhenNoObstacles);

            List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam);

            foreach (GridPos pos in resultPathList)
                Console.WriteLine(pos.x + "," + pos.y);

            //WorldGenerator.ResetWorld();

            //using (var game = new SimulationGame())
            //    game.Run();

            //Console.WriteLine(GeometryUtils.getPositionWithinChunk(-1, -1, 32, 32));

            /*var stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                GeometryUtils.getChunkPositionOld(-32, -32, 32, 32);
            }

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();

            stopwatch.Reset();

            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                GeometryUtils.getChunkPosition(-32, -32, 32, 32);
            }
            
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);*/



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

            //var a = World.getTouchedWorldBlocksCoordinates(new Rectangle(32, 32, 32, 32)).ToArray();

            //foreach (var b in a)
            //{
            //    Console.WriteLine(b);
            //}
        }
    }
#endif
}
