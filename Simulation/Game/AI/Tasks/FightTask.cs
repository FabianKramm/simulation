using Microsoft.Xna.Framework;
using Simulation.Game.AI.Evaluation;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Enums;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.Game.AI.AITasks
{
    public class FightTask: BehaviorTask
    {
        private TaskRater taskRater = new TaskRater();

        public FightTask(LivingEntity livingEntity): base(livingEntity) { }

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
        public override void Update(GameTime gameTime)
        {
            if(subject.Skills.Length > 0)
            {
                Circle circle = new Circle((int)subject.Position.X, (int)subject.Position.Y, subject.AttentionRadius);
                List<LivingEntity> hittedEntities = CollisionUtils.GetLivingHittedObjects(circle, subject.InteriorID, subject, (int)FractionRelationType.HOSTILE);

                if (hittedEntities.Count > 0)
                {
                    foreach(var hittedEntity in hittedEntities)
                    {
                        

                        foreach (var skill in subject.Skills)
                        {
                            if (skill.IsReady() == false)
                                continue;

                            if (skill is FireballSkill)
                            {

                            }

                            if (skill is SlashSkill)
                            {

                            }
                        }
                    }
                }
            }

            setFailed();
        }
    }
}
