using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Simulation.Game.World
{
    public class InteriorManager
    {
        private ConcurrentDictionary<string, Interior> loadedInteriors = new ConcurrentDictionary<string, Interior>();
        private NamedLock interiorLocks = new NamedLock();

        private TimeSpan timeSinceLastGarbageCollect = TimeSpan.Zero;
        private static TimeSpan garbageCollectInterval = TimeSpan.FromSeconds(30);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void loadInterior(string interiorID)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                GameConsole.WriteLine("ChunkLoading", "Interior " + interiorID + " loaded in main thread");
            }

            loadedInteriors[interiorID] = WorldLoader.LoadInterior(interiorID);
        }

        public int GetLoadedInteriorAmount()
        {
            return loadedInteriors.Count;
        }

        public void SaveInteriorAsync(Interior interior)
        {
            Task.Run(() =>
            {
                interiorLocks.Enter(interior.ID);

                try
                {
                    WorldLoader.SaveInterior(interior);
                }
                finally
                {
                    interiorLocks.Exit(interior.ID);
                }
            });
        }

        public void LoadInteriorGuarded(string interiorID)
        {
            if (interiorID == Interior.Outside) return;

            interiorLocks.Enter(interiorID);

            try
            {
                if (loadedInteriors.ContainsKey(interiorID) == false)
                    loadInterior(interiorID);
            }
            finally
            {
                interiorLocks.Exit(interiorID);
            }
        }

        public Interior GetInterior(string interiorID)
        {
            if (interiorID == Interior.Outside) return null;

            interiorLocks.Enter(interiorID);

            try
            {
                if (loadedInteriors.ContainsKey(interiorID) == false)
                    loadInterior(interiorID);

                return loadedInteriors[interiorID];
            }
            finally
            {
                interiorLocks.Exit(interiorID);
            }
        }

        public bool IsInteriorLoaded(string interiorID)
        {
            return loadedInteriors.ContainsKey(interiorID);
        }

        private void garbageCollectInteriors()
        {
            ThreadingUtils.assertMainThread();
            int interiorsUnloaded = 0;
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            foreach (var interiorItem in loadedInteriors)
            {
                var key = interiorItem.Key;

                interiorLocks.Enter(key);

                try
                {
                    var found = false;

                    foreach (var durableEntity in SimulationGame.World.DurableEntities)
                    {
                        if (key == durableEntity.Value.InteriorID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Interior removedInterior;

                        loadedInteriors.TryRemove(key, out removedInterior);

                        if (removedInterior.AmbientObjects != null)
                            foreach (var ambientObject in removedInterior.AmbientObjects)
                                ambientObject.Destroy();

                        if (removedInterior.ContainedObjects != null)
                            foreach (var containedEntity in removedInterior.ContainedObjects)
                                containedEntity.Destroy();

                        // Save async
                        SaveInteriorAsync(removedInterior);

                        interiorsUnloaded++;
                    }
                }
                finally
                {
                    interiorLocks.Exit(key);
                }
            }

            stopwatch.Stop();

            if (interiorsUnloaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + interiorsUnloaded + " interiors took " + stopwatch.ElapsedMilliseconds);
            }
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastGarbageCollect += gameTime.ElapsedGameTime;

            if (timeSinceLastGarbageCollect > garbageCollectInterval)
            {
                timeSinceLastGarbageCollect = TimeSpan.Zero;
                garbageCollectInteriors();
            }

            foreach(var interiorItem in loadedInteriors)
            {
                interiorItem.Value.Update(gameTime);
            }
        }
    }
}
