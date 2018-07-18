using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public abstract class WorldPartManager<KEY, PART>
    {
        protected TimeSpan garbageCollectInterval;
        protected TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;

        private ConcurrentDictionary<KEY, PART> loadedParts = new ConcurrentDictionary<KEY, PART>();
        private NamedLock<KEY> partLocks = new NamedLock<KEY>();

        protected abstract PART loadUnguarded(KEY key);
        protected abstract void saveUnguarded(KEY key, PART part);

        protected abstract bool shouldRemoveDuringGarbageCollection(KEY key, PART part);
        protected abstract void unloadPart(KEY key, PART part);

        public WorldPartManager(TimeSpan garbageCollectInterval)
        {
            this.garbageCollectInterval = garbageCollectInterval;
        }

        public int CountLoaded()
        {
            return loadedParts.Count;
        }

        public ICollection<KEY> GetKeys()
        {
            return loadedParts.Keys;
        }

        public void LoadGuarded(KEY key)
        {
            partLocks.Enter(key);

            try
            {
                if (loadedParts.ContainsKey(key) == false)
                    loadedParts[key] = loadUnguarded(key);
            }
            finally
            {
                partLocks.Exit(key);
            }
        }

        public void LoadAsync(KEY key)
        {
            Task.Run(() =>
            {
                LoadGuarded(key);
            });
        }

        protected void SaveAsync(KEY key, PART part)
        {
            Task.Run(() =>
            {
                partLocks.Enter(key);

                try
                {
                    saveUnguarded(key, part);
                }
                finally
                {
                    partLocks.Exit(key);
                }
            });
        }

        public bool IsLoaded(KEY key)
        {
            return loadedParts.ContainsKey(key);
        }

        public void Set(KEY key, Action<PART> action, bool loadIfNotExists = true)
        {
            partLocks.Enter(key);

            try
            {
                if (loadedParts.ContainsKey(key) == false)
                {
                    if (loadIfNotExists)
                    {
                        loadedParts[key] = loadUnguarded(key);
                    }
                    else
                    {
                        return;
                    }
                }

                action(loadedParts[key]);
            }
            finally
            {
                partLocks.Exit(key);
            }
        }

        public PART Get(KEY key, bool loadIfNotExists = true)
        {
            partLocks.Enter(key);

            try
            {
                if (loadedParts.ContainsKey(key) == false)
                {
                    if (loadIfNotExists)
                    {
                        loadedParts[key] = loadUnguarded(key);
                    }
                    else
                    {
                        return default(PART);
                    }
                }

                return loadedParts[key];
            }
            finally
            {
                partLocks.Exit(key);
            }
        }

        private void garbageCollect()
        {
            ThreadingUtils.assertMainThread();

            var partsUnloaded = 0;
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            foreach (var part in loadedParts)
            {
                var key = part.Key;

                partLocks.Enter(key);

                try
                {
                    var shouldRemove = shouldRemoveDuringGarbageCollection(part.Key, part.Value);

                    if (shouldRemove)
                    {
                        PART removedPart;

                        loadedParts.TryRemove(key, out removedPart);

                        unloadPart(part.Key, part.Value);

                        // Save async
                        SaveAsync(part.Key, removedPart);

                        partsUnloaded++;
                    }
                }
                finally
                {
                    partLocks.Exit(key);
                }
            }

            stopwatch.Stop();

            if (partsUnloaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + partsUnloaded + " " + GetType().Name + " took " + stopwatch.ElapsedMilliseconds);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            timeSinceLastGarbageCollect += gameTime.ElapsedGameTime;

            if (timeSinceLastGarbageCollect > garbageCollectInterval)
            {
                timeSinceLastGarbageCollect = TimeSpan.Zero;
                garbageCollect();
            }
        }
    }
}
