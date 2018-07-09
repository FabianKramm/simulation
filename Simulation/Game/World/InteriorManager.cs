using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.World.Generator;
using Simulation.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
            loadedInteriors[interiorID] = WorldLoader.loadInterior(interiorID);
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
                    WorldLoader.saveInterior(interior);
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

            foreach (var interiorItem in loadedInteriors)
            {
                var key = interiorItem.Key;

                interiorLocks.Enter(key);

                try
                {
                    var found = false;

                    foreach (var durableEntity in SimulationGame.World.durableEntities)
                    {
                        if (key == durableEntity.Value.InteriorID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Interior removedItem;

                        loadedInteriors.TryRemove(key, out removedItem);

                        // Save async
                        SaveInteriorAsync(removedItem);

                        interiorsUnloaded++;
                    }
                }
                finally
                {
                    interiorLocks.Exit(key);
                }
            }

            if (interiorsUnloaded > 0)
            {
                GameConsole.WriteLine("ChunkLoading", "Garbage Collector unloaded " + interiorsUnloaded + " interiors");
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
        }
    }
}
