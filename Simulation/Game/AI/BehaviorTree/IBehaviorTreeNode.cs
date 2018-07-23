using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.BehaviorTree
{
    public interface IBehaviorTreeNode
    {
        void Reset();

        BehaviourTreeStatus Tick(GameTime gameTime);
    }
}
