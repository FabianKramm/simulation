using System;
using System.Diagnostics;
using System.Threading;

namespace Simulation.Util
{
    public class ThreadingUtils
    {
        public static void assertChildThread()
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId != 1, "Method should only be called in child thread");
        }

        public static void assertMainThread()
        {
            Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1, "Method should only be called in main thread");
        }
    }
}
