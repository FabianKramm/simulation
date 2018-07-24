using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.TaskRater
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
