using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public abstract class ParentBaseNode: IParentBehaviorTreeNode
    {
        protected RootNode rootNode;

        public ParentBaseNode(RootNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public virtual void ExchangeRootNode(RootNode newRootNode)
        {
            this.rootNode = newRootNode;
        }

        public abstract void AddChild(IBehaviorTreeNode child);
        public abstract void Reset();
        public abstract BehaviourTreeStatus Tick(GameTime gameTime);
    }
}
