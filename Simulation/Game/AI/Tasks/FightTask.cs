using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree;
using Simulation.Game.AI.Evaluation;
using Simulation.Game.AI.Tasks;
using Simulation.Game.Effects;
using Simulation.Game.Fractions;
using Simulation.Game.MetaData;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Scripts.Skills;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;

namespace Simulation.Game.AI.AITasks
{
    public class FightTask: BehaviorTask
    {
        private TaskRater taskRater;
        private BehaviorTask activeTask = null;
        private string activeTaskId = null;

        public FightTask(LivingEntity livingEntity): base(livingEntity) { }

        protected override BehaviourTreeStatus internalUpdate(GameTime gameTime)
        {
            if(subject.Skills.Length > 0)
            {
                var attentionBlockRadius = LivingEntityType.lookup[subject.LivingEntityType].AttentionBlockRadius;
                var circle = new Circle((int)subject.Position.X, (int)subject.Position.Y, attentionBlockRadius * WorldGrid.BlockSize.X);
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
                        {
                            taskRater.AddTask(FollowTask.ID + hittedEntity.ID, () => new FollowTask((MovingEntity)subject, hittedEntity.ID, WorldGrid.BlockSize.X), 100 - (distance / WorldGrid.BlockSize.X) + -aggro);
                            taskRater.AddTask(BlinkTask.ID + hittedEntity.ID, () => new BlinkTask(subject, hittedEntity.Position), 101 - (distance / WorldGrid.BlockSize.X) + -aggro);
                        }

                        if ((float)subject.CurrentLife / (float)subject.MaximumLife < 0.25f)
                            taskRater.AddTask(FleeTask.ID + hittedEntity.ID, () => new FleeTask((MovingEntity)subject, hittedEntity, 20 * WorldGrid.BlockSize.X), 1000 - (distance / WorldGrid.BlockSize.X) + -aggro);
                    }

                    if(taskRater.HasTask() == false)
                    {
                        activeTask = null;
                        activeTaskId = null;
                    }

                    while (taskRater.HasTask())
                    {
                        var highestTask = taskRater.GetHighestRanked();

                        if (highestTask.TaskIdentifier != activeTaskId)
                        {
                            activeTask = highestTask.TaskCreator();
                            activeTaskId = highestTask.TaskIdentifier;
                        }

                        if (activeTask.Status == BehaviourTreeStatus.Running)
                        {
                            activeTask.Update(gameTime);

                            if (activeTask.Status == BehaviourTreeStatus.Failure)
                            {
                                continue;
                            }
                        }

                        break;
                    }

                    if(enemyInSight == true)
                        return BehaviourTreeStatus.Running;
                }
            }

            return BehaviourTreeStatus.Failure;
        }
    }
}
