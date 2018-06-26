using System;
using System.Threading;

namespace Simulation.Util
{
    public class ThreadingUtils
    {
        public static void assertChildThread()
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                throw new Exception("Method execution not allowed in main thread");
            }
        }

        public static void assertMainThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                throw new Exception("Method execution not allowed in child thread");
            }
        }
    }
}
