using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;

namespace Simulation.Game.AI
{
    public class GuardAI: BaseAI
    {
        public WorldPosition BlockGuardPosition;
        public Vector2 LookDirection;

        public GuardAI(MovingEntity movingEntity, WorldPosition blockGuardPosition, Vector2 lookDirection) : base(movingEntity)
        {
            BlockGuardPosition = blockGuardPosition;
            LookDirection = lookDirection;

            behaviorTree = createBehaviorTree();
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    .LongRunningResultCached(() => new GoToTask(Entity, BlockGuardPosition))
                    .SingleStep((GameTime gameTime) => {
                        Entity.Direction = LookDirection;

                        return BehaviourTreeStatus.Success;
                    })
                .End()
                .Build(),
                Entity
            );
        }
    }
}
