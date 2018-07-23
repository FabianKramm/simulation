using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class RootNode: IParentBehaviorTreeNode
    {
        private IBehaviorTreeNode childNode;
        private List<LongRunningActionNode> blockingRunningNodes = new List<LongRunningActionNode>();

        private bool forceTreeTick = true;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private TimeSpan tickFrequency; // Only tick in this frequency when a task is busy

        public RootNode(TimeSpan? tickFrequency = null)
        {
            this.tickFrequency = tickFrequency ?? TimeSpan.FromMilliseconds(300);
        }

        public void AddChild(IBehaviorTreeNode child)
        {
            childNode = child;
        }

        public void AddBlockingNode(LongRunningActionNode blockingNode)
        {
            blockingRunningNodes.Add(blockingNode);
        }

        public void Reset()
        {
            elapsedTime = TimeSpan.Zero;
            forceTreeTick = true;

            blockingRunningNodes = new List<LongRunningActionNode>();
            childNode.Reset();
        }

        public BehaviourTreeStatus Tick(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if(forceTreeTick || elapsedTime >= tickFrequency)
            {
                forceTreeTick = false;

                if(blockingRunningNodes.Count > 0)
                {
                    for(int i=0;i<blockingRunningNodes.Count;i++)
                    {
                        var status = blockingRunningNodes[i].Tick(new GameTime(gameTime.TotalGameTime, elapsedTime));

                        if (status != BehaviourTreeStatus.Running)
                        {
                            blockingRunningNodes.RemoveAt(i);
                            i--;
                        }
                    }

                    elapsedTime = TimeSpan.Zero;

                    return BehaviourTreeStatus.Running;
                }
                else
                {
                    var outcome = childNode.Tick(new GameTime(gameTime.TotalGameTime, elapsedTime));

                    elapsedTime = TimeSpan.Zero;

                    if (outcome != BehaviourTreeStatus.Running)
                    {
                        Reset();
                    }

                    return outcome;
                }
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
