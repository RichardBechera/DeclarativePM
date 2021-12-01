using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.ConformanceModels;

namespace DeclarativePM.Lib.Utils
{
    public static class ActivationTreeBuilder
    {
        public static ActivationBinaryTree BuildTree(List<Event> trace, IBiTemplate constraint)
        {
            ActivationBinaryTree tree = new(constraint);
            int id = 1;
            foreach (var e in trace)
            {
                e.ActivityInTraceId = id;
                id++;
                foreach (var leaf in tree.Leaves.ToList())
                {
                    if (leaf.IsDead)
                        continue;

                    if (constraint.IsActivation(e))
                    {
                        //left
                        ActivationNode left = new(leaf.Subtrace.ToList());
                        tree.AddNodeLeft(leaf, left);
                        
                        //right
                        ActivationNode right = new(leaf.Subtrace.ToList());
                        right.Subtrace.Add(e);
                        tree.AddNodeRight(leaf, right);
                    }
                    else
                    {
                        leaf.Subtrace.Add(e);
                    }
                }
            }
            
            AssignLeavesStatus(tree.Leaves, constraint);

            return tree;
        }

        private static void AssignLeavesStatus(List<ActivationNode> nodes, IBiTemplate constraint)
        {
            foreach (var leaf in nodes)
            {
                leaf.IsDead = !MainMethods
                    .EvaluateExpression(leaf.Subtrace, constraint.GetExpression());
            }

            List<ActivationNode> maxFulfillingSubtraces = GetMaximalFulfillingSubtraces(nodes);
            foreach (var node in maxFulfillingSubtraces)
            {
                node.MaxFulfilling = true;
            }
        }

        private static List<ActivationNode> GetMaximalFulfillingSubtraces(List<ActivationNode> nodes)
        {
            Queue<ActivationNode> fifo = new(nodes
                .Where(node => !node.IsDead)
                .OrderByDescending(x => x.Subtrace.Count));
            ActivationNode longest = fifo.Dequeue();
            List<ActivationNode> result = new();
            int counter = fifo.Count;
            
            while (fifo.Count > 0)
            {
                ActivationNode candidate = fifo.Dequeue();
                counter--;
                
                if (!IsSubtrace(new(longest.Subtrace), new(candidate.Subtrace)))
                    fifo.Enqueue(candidate);

                if (counter == 0)
                {
                    result.Add(longest);
                    fifo.TryDequeue(out longest);
                    counter = fifo.Count;
                }
            }
            if (longest is not null && !result.Contains(longest))
                result.Add(longest);

            return result;
        }

        /// <summary>
        /// Returns bool whether b is subtrace of a.
        /// </summary>
        /// <param name="a">Trace a</param>
        /// <param name="b">Trace b</param>
        /// <returns></returns>
        private static bool IsSubtrace(Stack<Event> a, Stack<Event> b)
        {
            if (a.Count < b.Count)
                return false;
            
            while (a.Count > 0 && b.Count > 0)
            {
                if (a.Peek().Activity.Equals(b.Peek().Activity) &&
                    a.Peek().ActivityInTraceId == b.Peek().ActivityInTraceId)
                {
                    a.Pop();
                    b.Pop();
                    continue;
                }

                a.Pop();
            }

            return b.Count == 0;
        }
    }
}