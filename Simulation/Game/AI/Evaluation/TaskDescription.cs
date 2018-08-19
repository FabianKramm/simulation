using Simulation.Game.AI.Tasks;
using System;

namespace Simulation.Game.AI.Evaluation
{
    public class TaskDescription
    {
        public string TaskIdentifier;
        public float Score;
        public Func<BehaviorTask> TaskCreator;

        public TaskDescription(string taskIdentifier, Func<BehaviorTask> taskCreator, float score)
        {
            TaskIdentifier = taskIdentifier;
            Score = score;
            TaskCreator = taskCreator;
        }
    }
}
