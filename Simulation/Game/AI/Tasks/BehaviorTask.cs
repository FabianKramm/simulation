using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;

namespace Simulation.Game.AI.Tasks
{
    public abstract class BehaviorTask
    {
        public BehaviourTreeStatus Status
        {
            get; private set;
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

        public void Update(GameTime gameTime)
        {
            if(Status == BehaviourTreeStatus.Running)
                Status = internalUpdate(gameTime);
        }

        protected abstract BehaviourTreeStatus internalUpdate(GameTime gameTime);
    }
}
