using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class SequenceNode: IParentBehaviorTreeNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();
        protected RootNode rootNode;

        public SequenceNode(RootNode rootNode)
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

        public BehaviourTreeStatus Tick(GameTime gameTime)
        {
            foreach(var child in children)
            {
                var childStatus = child.Tick(gameTime);

                if (childStatus != BehaviourTreeStatus.Success)
                {
                    if (child is LongRunningActionNode && ((LongRunningActionNode)child).IsBlocking && childStatus == BehaviourTreeStatus.Running)
                        rootNode.AddBlockingNode((LongRunningActionNode)child);

                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
