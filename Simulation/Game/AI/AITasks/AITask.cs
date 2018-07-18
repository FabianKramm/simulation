using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.AITasks
{
    public abstract class AITask
    {
        public bool IsFinished;
        public bool IsStarted
        {
            get; private set;
        }

        protected LivingEntity subject;

        public AITask(LivingEntity subject)
        {
            this.subject = subject;
        }

        public virtual void Start()
        {
            IsStarted = true;
        }

        public abstract void Update(GameTime gameTime);
    }
}
