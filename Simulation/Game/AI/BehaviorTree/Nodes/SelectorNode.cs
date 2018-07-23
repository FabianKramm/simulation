using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    /// <summary>
    /// Selects the first node that succeeds. Tries successive nodes until it finds one that doesn't fail.
    /// </summary>
    public class SelectorNode : IParentBehaviorTreeNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();
        protected RootNode rootNode;

        public SelectorNode(RootNode rootNode)
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
            foreach (var child in children)
            {
                var childStatus = child.Tick(time);
                if (childStatus != BehaviourTreeStatus.Failure)
                {
                    if (child is LongRunningActionNode && ((LongRunningActionNode)child).IsBlocking && childStatus == BehaviourTreeStatus.Running)
                        rootNode.AddBlockingNode((LongRunningActionNode)child);

                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Failure;
        }
    }
}
