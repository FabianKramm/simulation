using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simulation.Util
{
    public class NamedLock<T>
    {
        private class Lock
        {
            public int locks = 0;
            public object lockObj = new object();
        }

        private Dictionary<T, Lock> lockDir;

        public NamedLock()
        {
            lockDir = new Dictionary<T, Lock>();
        }

        public bool TryEnter(T key)
        {
            Lock lockObject;

            lock (lockDir)
            {
                if (lockDir.ContainsKey(key))
                {
                    return false;
                }
                else
                {
                    lockObject = new Lock();
                    lockObject.locks++;

                    lockDir[key] = lockObject;

                    Monitor.Enter(lockDir[key].lockObj);

                    return true;
                }
            }
        }

        public void Enter(T key)
        {
            Lock lockObject;
            bool isBlocked = false;

            lock (lockDir)
            {
                if (lockDir.ContainsKey(key))
                {
                    isBlocked = true;

                    lockObject = lockDir[key];
                    lockObject.locks++;
                }
                else
                {
                    lockObject = new Lock();
                    lockObject.locks++;

                    lockDir[key] = lockObject;

                    Monitor.Enter(lockDir[key].lockObj);
                }
            }

            if (isBlocked)
            {
                Monitor.Enter(lockDir[key].lockObj);
            }
        }

        public void Exit(T key)
        {
            lock (lockDir)
            {
                if(lockDir.ContainsKey(key))
                {
                    var lockObject = lockDir[key];

                    lockObject.locks--;

                    if (lockObject.locks == 0)
                    {
                        lockDir.Remove(key);
                    }

                    Monitor.Exit(lockObject.lockObj);
                }
            }
        }
    }
}
