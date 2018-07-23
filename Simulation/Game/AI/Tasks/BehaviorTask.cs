using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.Tasks
{
    public abstract class BehaviorTask
    {
        public BehaviourTreeStatus Status
        {
            get; protected set;
        } = BehaviourTreeStatus.Running;

        protected LivingEntity subject;

        public BehaviorTask(LivingEntity subject)
        {
            this.subject = subject;
        }

        public virtual void Start()
        {

        }

        public virtual void Destroy()
        {

        }

        public abstract void Update(GameTime gameTime);
    }
}
