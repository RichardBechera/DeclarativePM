using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Utils
{
    public static class MainMethods
    {
        /// <summary>
        /// Evaluates whether expression holds for the case.
        /// </summary>
        /// <param name="events">Sorted list of event in a case.</param>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="position">Starting position from which we start the evaluation.</param>
        /// <returns>Bool whether expression holds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Expression contains undefined operator.</exception>
        public static bool EvaluateExpression(List<Event> events, LtlExpression expression, int position = 0)
        {
            if (position >= events.Count)
                return false;
            
            switch (expression.Operator)
            {
                case Operators.None:
                    return events[position].Activity == expression.Atom;
                    
                case Operators.Not:
                    return !EvaluateExpression(events, expression.InnerLeft, position);
                    
                case Operators.Next:
                    if (position >= 0 && position < (events.Count - 1))
                    {
                        return EvaluateExpression(events, expression.InnerLeft, position + 1);
                    }

                    return false;
                    
                case Operators.Subsequent:
                    if (position >= 0 && position < (events.Count - 1))
                    {
                        return EvaluateExpression(events, expression.InnerLeft, position) &&
                               EvaluateExpression(events, expression, position + 1);
                    }

                    return EvaluateExpression(events, expression.InnerLeft, position);
                
                case Operators.Eventual:
                    if (position >= 0 && position < (events.Count - 1))
                    {
                        return EvaluateExpression(events, expression.InnerLeft, position) ||
                               EvaluateExpression(events, expression, position + 1);
                    }

                    return EvaluateExpression(events, expression.InnerLeft, position);
                    
                case Operators.And:
                    return EvaluateExpression(events, expression.InnerLeft, position) &&
                           EvaluateExpression(events, expression.InnerRight, position);
                    
                case Operators.Or:
                    return EvaluateExpression(events, expression.InnerLeft, position) ||
                           EvaluateExpression(events, expression.InnerRight, position);
                    
                case Operators.Imply:
                    return !EvaluateExpression(events, expression.InnerLeft, position) ||
                           EvaluateExpression(events, expression.InnerRight, position);
                    
                case Operators.Equivalence:
                    bool A = EvaluateExpression(events, expression.InnerLeft, position);
                    bool B = EvaluateExpression(events, expression.InnerRight, position);
                    return (A && B) || (!A && !B);
                
                case Operators.Least:
                    if (position >= 0 && position < (events.Count - 1))
                    {
                        return EvaluateExpression(events, expression.InnerRight, position) ||
                               (EvaluateExpression(events, expression.InnerLeft, position) &&
                                EvaluateExpression(events, expression, position + 1));
                    }

                    return EvaluateExpression(events, expression.InnerLeft, position) ||
                           EvaluateExpression(events, expression.InnerRight, position);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static List<Event> GetFulfillment(ActivationBinaryTree tree)
        {
            return tree.Leafs
                .Where(x => x.MaxFulfilling)
                .Select(x => x.Subtrace)
                .Aggregate((x, y) => x
                    .Intersect(y, new EventEqualityComparer())
                    .ToList())
                .Where(x => tree.Constraint.IsActivation(x))
                .ToList();
        }

        public static List<Event> GetViolation(ActivationBinaryTree tree)
        {
            var all = tree.Leafs
                .Where(x => x.MaxFulfilling)
                .SelectMany(x => x.Subtrace);
            return tree.Leafs
                .SelectMany(x => x.Subtrace)
                .Except(all)
                .Where(x => tree.Constraint.IsActivation(x))
                .ToList();
        }

        public static List<Event> GetConflict(ActivationBinaryTree tree)
        {
            return tree.Leafs
                .Where(x => x.MaxFulfilling)
                .SelectMany(x => x.Subtrace)
                .Except(GetViolation(tree))
                .Except(GetFulfillment(tree))
                .Where(x => tree.Constraint.IsActivation(x))
                .ToList();
        }

        public static List<ActivationNode> GetConflictNodes(ActivationBinaryTree tree, List<Event> conflicts = null)
        {
            conflicts ??= GetConflict(tree);
            return GetNodesWith(tree, conflicts).ToList();
        }
        
        public static List<ActivationNode> GetViolationNodes(ActivationBinaryTree tree, List<Event> violations = null)
        {
            violations ??= GetViolation(tree);
            return GetNodesWith(tree, violations).ToList();
        }
        
        public static List<ActivationNode> GetFulfillNodes(ActivationBinaryTree tree, List<Event> fulfillments = null)
        {
            fulfillments ??= GetFulfillment(tree);
            return GetNodesWith(tree, fulfillments).ToList();
        }

        private static IEnumerable<ActivationNode> GetNodesWith(ActivationBinaryTree tree, List<Event> intr)
            => tree.Leafs.Where(node => !node.IsDead
                                        && node.MaxFulfilling
                                        && node.Subtrace.Intersect(intr).Any());

        public static Dictionary<ActivationNode, double> LocalLikelyhood(ActivationBinaryTree tree,
            List<ActivationNode> conflictResolution = null)
        {
            List<Event> conflicts = GetConflict(tree);
            conflictResolution ??= GetConflictNodes(tree, conflicts);

            int na = conflictResolution
                .Aggregate(0, (num, c) => num + c.Subtrace
                    .Count(tree.Constraint.IsActivation));
            return conflictResolution
                .ToDictionary(x => x, y => LocalLikelyhoodNode(y, na, tree.Constraint));
        }

        private static double LocalLikelyhoodNode(ActivationNode node, int na, IBiTemplate constraint)
            => node.Subtrace.Count(constraint.IsActivation) / (double)na;

        //TODO
        public static double GlobalLikelyhood(ActivationBinaryTree tree,
            List<ParametrisedTemplate> model, ActivationNode conflictResolution)
        {
            double result = 0;
            List<Event> kOfConflictActivations = GetConflict(tree);
            List<IBiTemplate> constraints = model.Where(x => x.TemplateType == TemplateTypes.BiTemplate)
                .SelectMany(x => x.TemplateInstances).Cast<IBiTemplate>().ToList();
            foreach (Event resolution in kOfConflictActivations)
            {
                int gama = 0;
                foreach (var constraint in constraints)
                {
                    ActivationBinaryTree tempTree = ActivationTreeBuilder.BuildTree(conflictResolution.Subtrace, constraint);
                    gama += GetFulfillment(tempTree).Contains(resolution) ? 1 : 0;
                }

                result += gama;
            }

            return result / kOfConflictActivations.Count;
        }




    }
}