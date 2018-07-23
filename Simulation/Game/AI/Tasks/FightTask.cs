using Microsoft.Xna.Framework;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Objects.Entities;
using System;

namespace Simulation.Game.AI.AITasks
{
    public class FightTask: BehaviorTask
    {
        public FightTask(LivingEntity livingEntity): base(livingEntity) { }

        public override void Update(GameTime gameTime)
        {
            /*
            * Wander AI:
            * Selector    
            *  FightTask
            *  Sequence
            *    WaitTask
            *    WanderTask
            * 
            * while(Fightable Entity is in radius) {
            *     Decide between actions
            *      MoveCloserToTarget
            *      Wait
            *      UseSkillIfInRange And Not Cooldown
            *      Flee
            *      
            *  Tick
            *      Class
            *          Score, TaskCreator
            *  
            *  
            * }
            */

            Status = BehaviorTree.BehaviourTreeStatus.Failure;
        }
    }
}
