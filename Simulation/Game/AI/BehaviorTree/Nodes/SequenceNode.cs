using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class SequenceNode: ParentBaseNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

        public SequenceNode(RootNode rootNode): base(rootNode) { }

        public override void ExchangeRootNode(RootNode newRootNode)
        {
            base.ExchangeRootNode(newRootNode);

            foreach (var child in children)
            {
                if (child is ParentBaseNode)
                    ((ParentBaseNode)child).ExchangeRootNode(newRootNode);
            }
        }

        public override void AddChild(IBehaviorTreeNode child)
        {
            children.Add(child);
        }

        public override void Reset()
        {
            foreach (var child in children)
            {
                child.Reset();
            }
        }

        public override BehaviourTreeStatus Tick(GameTime gameTime)
        {
            for(int i=0;i<children.Count;i++) 
            {
                var child = children[i];
                var childStatus = child.Tick(gameTime);

                if (childStatus != BehaviourTreeStatus.Success)
                {
                    if (child is LongRunningActionNode && ((LongRunningActionNode)child).IsBlocking && childStatus == BehaviourTreeStatus.Running)
                        rootNode.AddBlockingNode((LongRunningActionNode)child);

                    if (childStatus == BehaviourTreeStatus.Running)
                        for(int j=i+1;j<children.Count;j++)
                        {
                            children[j].Reset();
                        }

                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
