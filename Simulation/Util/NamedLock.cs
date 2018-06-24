using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simulation.Util
{
    public class NamedLock
    {
        private class Lock
        {
            public int locks = 0;
            public object lockObj = new object();
        }

        private Dictionary<string, Lock> lockDir;

        public NamedLock()
        {
            lockDir = new Dictionary<string, Lock>();
        }

        public bool TryEnter(string key)
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

        public void Enter(string key)
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

        public void Exit(string key)
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

        public Task executeGuardedAsync(string key, Action action)
        {
            return Task.Run(action);
        }

        public void executeGuardedSync(string key, Action action)
        {
            Enter(key);

            try
            {
                action();
            }
            finally
            {
                Exit(key);
            }
        }
    }
}
