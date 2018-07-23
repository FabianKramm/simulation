using Simulation.Game.AI.BehaviorTree.Nodes;

namespace Simulation.Game.AI.BehaviorTree
{
    public interface IParentBehaviorTreeNode: IBehaviorTreeNode
    {
        void AddChild(IBehaviorTreeNode child);
    }
}
