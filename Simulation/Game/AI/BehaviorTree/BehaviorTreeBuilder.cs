using Microsoft.Xna.Framework;
using Simulation.Game.AI.BehaviorTree.Nodes;
using Simulation.Game.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Simulation.Game.AI.BehaviorTree
{
    public class BehaviorTreeBuilder
    {
        /// <summary>
        /// Last node created.
        /// </summary>
        private IBehaviorTreeNode curNode = null;

        private RootNode rootNode;

        /// <summary>
        /// Stack node nodes that we are build via the fluent API.
        /// </summary>
        private Stack<IParentBehaviorTreeNode> parentNodeStack;

        public BehaviorTreeBuilder(TimeSpan? tickFrequency = null)
        {
            parentNodeStack = new Stack<IParentBehaviorTreeNode>();
            rootNode = new RootNode(tickFrequency);

            parentNodeStack.Push(rootNode);
        }

        /// <summary>
        /// Like an action node... but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviorTreeBuilder If(Func<GameTime, bool> fn)
        {
            var conditionalNode = new ConditionalNode(rootNode, fn);

            parentNodeStack.Peek().AddChild(conditionalNode);
            parentNodeStack.Push(conditionalNode);

            return this;
        }

        /// <summary>
        /// Like an action node... but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviorTreeBuilder Else()
        {
            ((ConditionalNode)parentNodeStack.Peek()).SwitchMode();

            return this;
        }

        /// <summary>
        /// Create a sequence node.
        /// </summary>
        public BehaviorTreeBuilder Sequence()
        {
            var sequenceNode = new SequenceNode(rootNode);

            parentNodeStack.Peek().AddChild(sequenceNode);
            parentNodeStack.Push(sequenceNode);

            return this;
        }

        /// <summary>
        /// Create a parallel node.
        /// </summary>
        public BehaviorTreeBuilder Parallel()
        {
            var parallelNode = new ParallelNode(rootNode);

            parentNodeStack.Peek().AddChild(parallelNode);
            parentNodeStack.Push(parallelNode);

            return this;
        }

        /// <summary>
        /// Create a selector node.
        /// </summary>
        public BehaviorTreeBuilder Selector()
        {
            var selectorNode = new SelectorNode(rootNode);

            parentNodeStack.Peek().AddChild(selectorNode);
            parentNodeStack.Push(selectorNode);

            return this;
        }

        public BehaviorTreeBuilder SingleStep(Func<GameTime, BehaviourTreeStatus> action)
        {
            var singleStepActionNode = new SingleStepActionNode(action);

            parentNodeStack.Peek().AddChild(singleStepActionNode);

            return this;
        }

        public BehaviorTreeBuilder LongRunning(Func<BehaviorTask> taskCreator)
        {
            var longRunningActionNode = new LongRunningActionNode(taskCreator);

            parentNodeStack.Peek().AddChild(longRunningActionNode);

            return this;
        }

        public BehaviorTreeBuilder LongRunningBlocking(Func<BehaviorTask> taskCreator)
        {
            var longRunningActionNode = new LongRunningActionNode(taskCreator, true);

            parentNodeStack.Peek().AddChild(longRunningActionNode);

            return this;
        }

        /// <summary>
        /// Splice a sub tree into the parent tree.
        /// </summary>
        public BehaviorTreeBuilder Splice(IBehaviorTreeNode subTree)
        {
            Debug.Assert(subTree != null, "Subtree is null");
            Debug.Assert(subTree is RootNode, "Subtree has to start with a root node");

            parentNodeStack.Peek().AddChild(((RootNode)subTree).ExchangeRootNode(rootNode));

            return this;
        }


        /// <summary>
        /// Build the actual tree.
        /// </summary>
        public IBehaviorTreeNode Build()
        {
            return rootNode;
        }

        /// <summary>
        /// Ends a sequence of children.
        /// </summary>
        public BehaviorTreeBuilder End()
        {
            curNode = parentNodeStack.Pop();
            return this;
        }
    }
}
