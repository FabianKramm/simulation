using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using System;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    
    public class LongRunningActionNode: IBehaviorTreeNode
    {
        /*
         * A long running action can be blocking which means no nodes before will be executed again (rather the node is added to the root node and the root node will tick it till its not running anymore) or 
         * when non blocking all nodes before are executed the taskInstance itself will remain the same until it is non running
         */
        public bool IsBlocking
        {
            get; private set;
        } = false;

        /*
         * If the result is cached of a long running action node
         */
        public bool IsResultCached
        {
            get; private set;
        } = false;

        private BehaviorTask taskInstance;
        private Func<BehaviorTask> taskCreator;

        public LongRunningActionNode(Func<BehaviorTask> taskCreator)
        {
            this.taskCreator = taskCreator;
        }

        public LongRunningActionNode(Func<BehaviorTask> taskCreator, bool isBlocking, bool isResultCached)
        {
            this.taskCreator = taskCreator;
            this.IsBlocking = isBlocking;
            this.IsResultCached = isResultCached;
        }

        public void Reset()
        {
            if(taskInstance != null)
            {
                taskInstance.Destroy();
                taskInstance = null;
            }
        }

        public BehaviourTreeStatus Tick(GameTime gameTime)
        {
            if (taskInstance == null)
            {
                taskInstance = taskCreator();
                taskInstance.Start();
            }

            if(taskInstance.Status == BehaviourTreeStatus.Running)
            {
                taskInstance.Update(gameTime);
            }

            var retStatus = taskInstance.Status;

            if(!IsResultCached && retStatus != BehaviourTreeStatus.Running)
            {
                Reset();
            }

            return retStatus;
        }
    }
}
