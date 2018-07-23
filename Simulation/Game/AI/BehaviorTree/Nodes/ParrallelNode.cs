using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class ParallelNode: IParentBehaviorTreeNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();
        protected RootNode rootNode;

        public ParallelNode(RootNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public void AddChild(IBehaviorTreeNode child)
        {
            children.Add(child);
        }

        public void Reset()
        {
            foreach (var child in children)
            {
                child.Reset();
            }
        }

        public BehaviourTreeStatus Tick(GameTime time)
        {
            var numChildrenSuceeded = 0;
            var numChildrenFailed = 0;

            foreach (var child in children)
            {
                var childStatus = child.Tick(time);

                switch (childStatus)
                {
                    case BehaviourTreeStatus.Success: ++numChildrenSuceeded; break;
                    case BehaviourTreeStatus.Failure: ++numChildrenFailed; break;
                    case BehaviourTreeStatus.Running:
                        if (child is LongRunningActionNode && ((LongRunningActionNode)child).IsBlocking)
                            rootNode.AddBlockingNode((LongRunningActionNode)child);
                        break;
                }
            }

            if (children.Count > numChildrenFailed + numChildrenSuceeded)
            {
                return BehaviourTreeStatus.Running;
            }

            return numChildrenSuceeded >= numChildrenFailed ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}
