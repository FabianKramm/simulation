using Microsoft.Xna.Framework;
using Simulation.Game.Hud;
using Simulation.Game.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Simulation.Game.World
{
    public class InteriorManager: WorldPartManager<string, Interior>
    {
        public InteriorManager(): base(TimeSpan.FromSeconds(30)) { }

        protected override Interior loadUnguarded(string key)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
            {
                GameConsole.WriteLine("ChunkLoading", "Interior " + key + " loaded in main thread");
            }

            var interior = WorldLoader.LoadInterior(key);

            interior.Connected = true;

            return interior;
        }

        protected override void saveUnguarded(string key, Interior part)
        {
            WorldLoader.SaveInterior(part);
        }

        protected override bool shouldRemoveDuringGarbageCollection(string key, Interior part)
        {
            foreach (var durableEntity in SimulationGame.World.DurableEntities)
            {
                if (key == durableEntity.Value.InteriorID)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void unloadPart(string key, Interior part)
        {
            if (part.AmbientObjects != null)
                foreach (var ambientObject in part.AmbientObjects)
                    ambientObject.Destroy();

            if (part.ContainedObjects != null)
                foreach (var containedEntity in part.ContainedObjects)
                    containedEntity.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ICollection<string> keys = GetKeys();

            for (int i = 0; i < keys.Count; i++)
            {
                var interiorItem = Get(keys.ElementAt(i), false);

                if (interiorItem != null)
                {
                    interiorItem.Update(gameTime);
                }
            }
        }
    }
}
