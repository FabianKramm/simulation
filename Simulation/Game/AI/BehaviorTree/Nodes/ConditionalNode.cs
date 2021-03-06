﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Simulation.Game.AI.BehaviorTree.Nodes
{
    public class ConditionalNode : ParentBaseNode
    {
        public bool AddIfChild = true;

        private Func<GameTime, bool> condition;
        private IBehaviorTreeNode ifTrue;
        private IBehaviorTreeNode ifFalse;

        public ConditionalNode(RootNode rootNode, Func<GameTime, bool> condition): base(rootNode)
        {
            this.condition = condition;
        }

        public void SwitchMode()
        {
            Debug.Assert(AddIfChild == true, "Mode was already switched!");

            AddIfChild = false;
        }
        
        public void SetIfTrue(IBehaviorTreeNode child)
        {
            Debug.Assert(ifTrue == null, "ifTrue was already set!");

            ifTrue = child;
        }

        public void SetIfFalse(IBehaviorTreeNode child)
        {
            Debug.Assert(ifFalse == null, "ifFalse was already set!");

            ifFalse = child;
        }

        public override void AddChild(IBehaviorTreeNode child)
        {
            if (AddIfChild)
            {
                ifTrue = child;
            }
            else
            {
                ifFalse = child;
            }
        }

        public override void Reset()
        {
            ifTrue.Reset();

            if (ifFalse != null)
                ifFalse.Reset();
        }

        public override BehaviourTreeStatus Tick(GameTime gameTime)
        {
            if(condition(gameTime))
            {
                if(ifFalse != null)
                    ifFalse.Reset();

                var status = ifTrue.Tick(gameTime);

                if (ifTrue is LongRunningActionNode && ((LongRunningActionNode)ifTrue).IsBlocking && status == BehaviourTreeStatus.Running)
                    rootNode.AddBlockingNode((LongRunningActionNode)ifTrue);

                return status;
            }
            else
            {
                ifTrue.Reset();

                var status = ifFalse != null ? ifFalse.Tick(gameTime) : BehaviourTreeStatus.Failure;

                if (ifFalse != null && ifFalse is LongRunningActionNode && ((LongRunningActionNode)ifFalse).IsBlocking && status == BehaviourTreeStatus.Running)
                    rootNode.AddBlockingNode((LongRunningActionNode)ifFalse);

                return status;
            }
        }

        public override void ExchangeRootNode(RootNode newRootNode)
        {
            base.ExchangeRootNode(newRootNode);

            if (ifTrue is ParentBaseNode)
            {
                ((ParentBaseNode)ifTrue).ExchangeRootNode(newRootNode);
            }

            if (ifFalse != null && ifFalse is IParentBehaviorTreeNode)
            {
                ((ParentBaseNode)ifFalse).ExchangeRootNode(newRootNode);
            }
        }
    }
}
