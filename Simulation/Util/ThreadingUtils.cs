using System;
using System.Collections.Generic;
using System.Threading;

namespace Simulation.Util
{
    public class ThreadingUtils
    {
        public static void checkIfMainThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                throw new Exception("Method not allowed from child Thread");
            }
        }
    }
}
