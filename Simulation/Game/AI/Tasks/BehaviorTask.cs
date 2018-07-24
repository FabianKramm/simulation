using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;

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

        protected void setSuccessful()
        {
            Status = BehaviourTreeStatus.Success;
        }

        protected void setFailed()
        {
            Status = BehaviourTreeStatus.Failure;
        }

        public abstract void Update(GameTime gameTime);
    }
}
