using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class ParallelNode: ParentBaseNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

        public ParallelNode(RootNode rootNode): base(rootNode) { }

        public override void AddChild(IBehaviorTreeNode child)
        {
            children.Add(child);
        }

        public override void ExchangeRootNode(RootNode newRootNode)
        {
            base.ExchangeRootNode(newRootNode);

            foreach (var child in children)
            {
                if (child is ParentBaseNode)
                    ((ParentBaseNode)child).ExchangeRootNode(newRootNode);
            }
        }

        public override void Reset()
        {
            foreach (var child in children)
            {
                child.Reset();
            }
        }

        public override BehaviourTreeStatus Tick(GameTime time)
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
