using System;
using Microsoft.Xna.Framework;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class SingleStepActionNode: IBehaviorTreeNode
    {
        /*
         * If the result is cached of a long running action node
         */
        public bool IsResultCached
        {
            get; private set;
        } = false;

        public BehaviourTreeStatus? cachedResult = null;

        private Func<GameTime, BehaviourTreeStatus> action;

        public SingleStepActionNode(Func<GameTime, BehaviourTreeStatus> action)
        {
            this.action = action;
        }

        public SingleStepActionNode(Func<GameTime, BehaviourTreeStatus> action, bool isResultCached)
        {
            this.action = action;
            this.IsResultCached = isResultCached;
        }

        public void Reset()
        {
            if(IsResultCached)
                cachedResult = null;
        }

        public BehaviourTreeStatus Tick(GameTime gameTime)
        {
            if(IsResultCached)
            {
                if(cachedResult == null)
                {
                    cachedResult = action(gameTime);
                }

                return cachedResult ?? BehaviourTreeStatus.Failure;
            }
            else
            {
                // We map running to failed for this kind of actions!
                return action(gameTime) == BehaviourTreeStatus.Success ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
            }
        }
    }
}
