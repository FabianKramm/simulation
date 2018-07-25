using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using System;

namespace Simulation.Game.AI.Evaluation
{
    public class TaskDescription
    {
        public string TaskIdentifier;
        public float Score;
        public Func<GameTime, BehaviorTask> TaskCreator;

        public TaskDescription(string taskIdentifier, Func<GameTime, BehaviorTask> taskCreator, float score)
        {
            TaskIdentifier = taskIdentifier;
            Score = score;
            TaskCreator = taskCreator;
        }
    }
}
