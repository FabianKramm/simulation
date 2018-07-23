using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using System;

namespace Simulation.Game.AI.UtilitySystem
{
    public class UtilitySystem
    {
        public float HighestScore { get; private set; } = float.NegativeInfinity;
        public string TaskIdentifier { get; private set; } = null;
        public Func<GameTime, BehaviorTask> TaskCreator { get; private set; } = null;

        public void AddTask(float score, Func<GameTime, BehaviorTask> taskCreator, string taskIdentifier = null)
        {
            if(score > HighestScore)
            {
                HighestScore = score;
                TaskCreator = taskCreator;
                TaskIdentifier = taskIdentifier;
            }
        }
    }
}
