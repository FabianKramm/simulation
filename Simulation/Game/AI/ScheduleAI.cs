using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.AI
{
    public class ScheduleAI : BaseAI
    {
        public class ScheduleTask
        {
            public int StartHour;
            public int StartMinute;

            public WorldPosition Position;


            public BehaviorTask Task;
        }

        private SortedList<int, ScheduleTask> schedule;

        public ScheduleAI(MovingEntity movingEntity, List<ScheduleTask> schedule): base(movingEntity)
        {
            this.schedule = new SortedList<int, ScheduleTask>();

            foreach (var task in schedule)
                this.schedule.Add(task.StartHour * 60 + task.StartMinute, task);
        }

        protected override IBehaviorTreeNode createBehaviorTree()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();

            return AIExtensions.WithFightingAI(
                builder
                .Sequence()
                    
                .End()
                .Build(),
                Entity
            );
        }
    }
}
