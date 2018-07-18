using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System.Collections.Generic;

namespace Simulation.Game.AI
{
    public abstract class BaseAI
    {
        public MovingEntity Entity
        {
            get; private set;
        }

        protected List<AITasks.AITask> tasksToProcess;

        public BaseAI(MovingEntity movingEntity)
        {
            Entity = movingEntity;
        }

        protected abstract void addAITasks();

        public virtual void Update(GameTime gameTime)
        {
            if (tasksToProcess == null || tasksToProcess.Count == 0)
            {
                addAITasks();
            }

            if (tasksToProcess != null && tasksToProcess.Count > 0)
            {
                if(!tasksToProcess[0].IsStarted)
                {
                    tasksToProcess[0].Start();
                }

                tasksToProcess[0].Update(gameTime);

                if(tasksToProcess[0].IsFinished)
                {
                    tasksToProcess.RemoveAt(0);
                }
            }
        }
    }
}
