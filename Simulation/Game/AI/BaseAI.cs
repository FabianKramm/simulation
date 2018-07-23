using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.Objects.Entities;
using System.Collections.Generic;

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
            behaviorTree.Tick(gameTime);
        }

        public void Init()
        {
            behaviorTree = createBehaviorTree();
        }

        protected abstract IBehaviorTreeNode createBehaviorTree();
    }
}
