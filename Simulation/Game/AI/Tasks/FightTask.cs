using Microsoft.Xna.Framework;
using Simulation.Game.AI.Evaluation;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Effects;
using Simulation.Game.Enums;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Skills;
using Simulation.Game.World;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;

namespace Simulation.Game.AI.AITasks
{
    public class FightTask: BehaviorTask
    {
        private TaskRater taskRater;
        private BehaviorTask activeTask = null;
        private string activeTaskId = null;

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
                var circle = new Circle((int)subject.Position.X, (int)subject.Position.Y, subject.AttentionBlockRadius * WorldGrid.BlockSize.X);
                var hittedEntities = CollisionUtils.GetLivingHittedObjects(circle, subject.InteriorID, subject, (int)FractionRelationType.HOSTILE);
                var subjectVectorPosition = subject.Position.ToVector();
                var enemyInSight = false;

                if (hittedEntities.Count > 0)
                {
                    taskRater = new TaskRater();

                    foreach(var hittedEntity in hittedEntities)
                    {
                        if (CollisionUtils.IsSightBlocked(subject, hittedEntity))
                            continue;

                        enemyInSight = true;

                        var hittedEntityVector = hittedEntity.Position.ToVector();
                        var getCloser = false;
                        var distance = GeometryUtils.GetEuclideanDistance(subject.Position, hittedEntity.Position);
                        var aggro = subject.GetAggroTowardsEntity(hittedEntity);

                        foreach (var skill in subject.Skills)
                        {
                            if (skill.IsReady() == false)
                                continue;

                            if (skill is FireballSkill)
                            {
                                if (distance < Fireball.MaxDistance && CollisionUtils.IsSightBlocked(subject, hittedEntity, 15) == false)
                                {
                                    skill.Use(hittedEntityVector);
                                }
                                else
                                {
                                    getCloser = true;
                                }
                            }

                            if (skill is SlashSkill)
                            {
                                if(distance < SlashSkill.Range)
                                {
                                    skill.Use(hittedEntityVector);
                                }
                                else
                                {
                                    getCloser = true;
                                }
                            }
                        }

                        if (getCloser)
                            taskRater.AddTask(FollowObjectTask.ID + hittedEntity.ID, (GameTime _gameTime) => new FollowObjectTask((MovingEntity)subject, hittedEntity, WorldGrid.BlockSize.X), 20 * WorldGrid.BlockSize.X - distance + aggro);
                    }

                    var highestTask = taskRater.GetHighestRanked();

                    if(highestTask != null)
                    {
                        if(highestTask.TaskIdentifier != activeTaskId)
                        {
                            activeTask = highestTask.TaskCreator(gameTime);
                            activeTaskId = highestTask.TaskIdentifier;
                        }

                        if(activeTask.Status == BehaviorTree.BehaviourTreeStatus.Running)
                            activeTask.Update(gameTime);
                    }
                    else
                    {
                        ((MovingEntity)subject).StopWalking();

                        activeTask = null;
                        activeTaskId = null;
                    }

                    if(enemyInSight == true)
                        return;
                }
            }

            setFailed();
        }
    }
}
