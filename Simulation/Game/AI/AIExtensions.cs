using Microsoft.Xna.Framework;
using Simulation.Game.AI.AITasks;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.BehaviorTree.Nodes;
using Simulation.Game.Objects.Entities;

namespace Simulation.Game.AI
{
    public class AIExtensions
    {
        public static IBehaviorTreeNode WithFightingAI(IBehaviorTreeNode tree, MovingEntity entity)
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder(((RootNode)tree).TickFrequency);

            return builder
                .Selector()
                    .LongRunning(() => new FightTask(entity))
                    .Sequence()
                        .SingleStepResultCached((GameTime gameTime) =>
                        {
                            entity.StopWalking();

                            return BehaviourTreeStatus.Success;
                        })
                        .Splice(tree)
                    .End()
                .End()
                .Build();
        }
    }
}
