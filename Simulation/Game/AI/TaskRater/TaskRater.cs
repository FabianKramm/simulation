using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Game.AI.TaskRater
{
    public class TaskRater
    {
        private Dictionary<string, TaskDescription> taskDictionary = new Dictionary<string, TaskDescription>();

        public void AddTask(string taskIdentifier, Func<GameTime, BehaviorTask> taskCreator, float initialScore = 0)
        {
            taskDictionary[taskIdentifier] = new TaskDescription(taskIdentifier, taskCreator, initialScore);
        }

        public void ModifyScore(string taskIdentifier, float modify)
        {
            Debug.Assert(taskDictionary.ContainsKey(taskIdentifier), "TaskDictionary is missing task - " + taskIdentifier);

            taskDictionary[taskIdentifier].Score += modify;
        }

        public TaskDescription GetHighestRanked()
        {
            TaskDescription highestRanked = null;

            foreach (var taskDesc in taskDictionary)
                if (highestRanked == null || taskDesc.Value.Score > highestRanked.Score)
                    highestRanked = taskDesc.Value;

            return highestRanked;
        }
    }
}
