using System;
using Microsoft.Xna.Framework;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class SingleStepActionNode: IBehaviorTreeNode
    {
        private Func<GameTime, BehaviourTreeStatus> action;

        public SingleStepActionNode(Func<GameTime, BehaviourTreeStatus> action)
        {
            this.action = action;
        }

        public void Reset()
        {
            // Noop bois
        }

        public BehaviourTreeStatus Tick(GameTime gameTime)
        {
            // We map running to failed for this kind of actions!
            return action(gameTime) == BehaviourTreeStatus.Success ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}
