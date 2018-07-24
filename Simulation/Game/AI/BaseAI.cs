using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Game.AI
{
    public abstract class BaseAI
    {
        protected IBehaviorTreeNode behaviorTree;

        public MovingEntity Entity
        {
            get; private set;
        }

        public BaseAI(MovingEntity movingEntity)
        {
            Entity = movingEntity;
        }

        public virtual void Update(GameTime gameTime)
        {
            Debug.Assert(behaviorTree != null, "BehaviorTree is null, did you forget to call init in the ai class?");

            behaviorTree.Tick(gameTime);
        }

        public void Init()
        {
            behaviorTree = createBehaviorTree();
        }

        protected abstract IBehaviorTreeNode createBehaviorTree();
    }
}
