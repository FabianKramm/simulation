using Simulation.Game.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Game.AI.Evaluation
{
    public class TaskRater
    {
        private Dictionary<string, TaskDescription> taskDictionary = new Dictionary<string, TaskDescription>();

        public void AddTask(string taskIdentifier, Func<BehaviorTask> taskCreator, float initialScore = 0)
        {
            if(taskDictionary.ContainsKey(taskIdentifier) == false)
            {
                taskDictionary[taskIdentifier] = new TaskDescription(taskIdentifier, taskCreator, initialScore);
            }
        }

        public void ModifyScore(string taskIdentifier, float modify)
        {
            Debug.Assert(taskDictionary.ContainsKey(taskIdentifier), "TaskDictionary is missing task - " + taskIdentifier);

            taskDictionary[taskIdentifier].Score += modify;
        }

        public bool HasTask()
        {
            return taskDictionary.Count > 0;
        }

        public TaskDescription GetHighestRanked()
        {
            TaskDescription highestRanked = null;
            string highestRankedKey = null;

            foreach (var taskDesc in taskDictionary)
                if (highestRanked == null || taskDesc.Value.Score > highestRanked.Score)
                {
                    highestRanked = taskDesc.Value;
                    highestRankedKey = taskDesc.Key;
                }
                    
            if(highestRankedKey != null)
            {
                taskDictionary.Remove(highestRankedKey);
            }

            return highestRanked;
        }
    }
}
