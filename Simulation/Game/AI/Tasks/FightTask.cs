using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI.AITasks
{
    public class FightTask
    {

        public void Update(GameTime gameTime)
        {
            /*
             * Wander AI:
             * Condition FightableEntity in Radius:
             *  FightTask
             * Else
             *  Sequence:
             *    WaitTask
             *    WanderTask
             * 
             * if(FightableEntity is in Radius) 
             *  WaitTask.Interupt();
             *  WaitTask = null;
             *  
             *  WanderTask.Interupt();
             *  WanderTask = null;
             *  
             *  FightTask.Update(FightableEntity)
             * else
             *  if(WaitTask == null)
             *      WaitTask = new WaitTask();
             *  if(WaitTask != Finished) 
             *      return WaitTask.Update();
             *      
             *  if(WanderTask == null)
             *      WanderTask = new WanderTask();
             *  if(WanderTask != Fnished)
             *      WanderTask.Update();
             *      
             *  if(WanderTask == Finished)
             *      WaitTask = null;
             *      WanderTask = null;
             * 
             * 
             * Every 350 ms
             * 
             * while(Fightable Entity is in radius) {
             *     Decide between actions
             *      MoveCloserToTarget
             *      Wait
             *      UseSkillIfInRange And Not Cooldown
             *      Flee
             * }
             */
        }
    }
}
