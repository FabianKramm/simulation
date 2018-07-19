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
        private ConcurrentDictionary<KEY, PART> loadedParts = new ConcurrentDictionary<KEY, PART>();

        protected TimeSpan garbageCollectInterval;
        protected TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;

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

        public void LoadAsync(KEY key)
        {
            Task.Run(() =>
            {
                loadedParts.GetOrAdd(key, this.loadUnguarded);
            });
        }

        protected void SaveAsync(KEY key, PART part)
        {
            Task.Run(() =>
            {
                saveUnguarded(key, part);
            });
        }

        public bool IsLoaded(KEY key)
        {
            return loadedParts.ContainsKey(key);
        }

        public PART Get(KEY key, bool loadIfNotExists = true)
        {
            if (loadIfNotExists)
            {
                return loadedParts.GetOrAdd(key, this.loadUnguarded);
            }
            else
            {
                PART value;

                loadedParts.TryGetValue(key, out value);

                return value;
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
                var shouldRemove = shouldRemoveDuringGarbageCollection(part.Key, part.Value);

                if (shouldRemove)
                {
                    PART removedPart;

                    bool couldRemove = loadedParts.TryRemove(key, out removedPart);

                    if (couldRemove)
                    {
                        unloadPart(key, removedPart);

                        // Save async
                        SaveAsync(key, removedPart);

                        partsUnloaded++;
                    }
                }
            }

            stopwatch.Stop();

            if (partsUnloaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + partsUnloaded + " " + GetType().Name + " parts took " + stopwatch.ElapsedMilliseconds + "ms");
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
