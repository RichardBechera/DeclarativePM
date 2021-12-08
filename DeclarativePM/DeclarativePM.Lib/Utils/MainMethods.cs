using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.ConformanceModels;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;

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
                    bool a = EvaluateExpression(events, expression.InnerLeft, position);
                    bool b = EvaluateExpression(events, expression.InnerRight, position);
                    return (a && b) || (!a && !b);
                
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

        public static bool EvaluateTemplate(List<Event> events, ITemplate template,
            bool preprocessing = true)
        {
            LtlExpression expr = template switch
            {
                AlternateResponse ar => ar.GetFinishingExpression(),
                AlternateSuccession asu => asu.GetFinishingExpression(),
                _ => template.GetExpression()
            };

            if (expr is null)
                return false;
            if(preprocessing)
                events = UtilMethods.PreprocessTraceForEvaluation(template, events);
            return EvaluateExpression(events, expr);
        }

        public static List<Event> GetFulfillment(ActivationBinaryTree tree)
        {
            return tree.Leaves
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
            var all = tree.Leaves
                .Where(x => x.MaxFulfilling)
                .SelectMany(x => x.Subtrace);
            return tree.Leaves
                .SelectMany(x => x.Subtrace)
                .Except(all)
                .Where(x => tree.Constraint.IsActivation(x))
                .ToList();
        }

        public static List<Event> GetConflict(ActivationBinaryTree tree, List<Event> violations = null, List<Event> fulfilments = null)

        {
            violations ??= GetViolation(tree);
            fulfilments ??= GetFulfillment(tree);
            return tree.Leaves
                .Where(x => x.MaxFulfilling)
                .SelectMany(x => x.Subtrace)
                .Except(violations)
                .Except(fulfilments)
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
            => tree.Leaves.Where(node => !node.IsDead
                                        && node.MaxFulfilling
                                        && node.Subtrace.Intersect(intr).Any());

        public static Dictionary<ActivationNode, double> LocalLikelihood(ActivationBinaryTree tree,
            List<ActivationNode> conflictResolution = null)
        {
            List<Event> conflicts = GetConflict(tree);
            conflictResolution ??= GetConflictNodes(tree, conflicts);

            int na = conflictResolution
                .Aggregate(0, (num, c) => num + c.Subtrace
                    .Count(tree.Constraint.IsActivation));
            return conflictResolution
                .ToDictionary(x => x, y => LocalLikelihoodNode(y, na, tree.Constraint));
        }
        
        public static double LocalLikelihood(ActivationBinaryTree tree,
            ActivationNode conflictResolution)
        {
            ActivationBinaryTree tree2 = ActivationTreeBuilder.BuildTree(conflictResolution.Subtrace, tree.Constraint);
            int nf = GetFulfillment(tree2).Count(e => conflictResolution.Subtrace.Contains(e));
            int na = conflictResolution.Subtrace.Count(tree.Constraint.IsActivation);

            return nf / (double) na;
        }

        private static double LocalLikelihoodNode(ActivationNode node, int na, BiTemplate constraint)
            => node.Subtrace.Count(constraint.IsActivation) / (double)na;

        //TODO
        public static double GlobalLikelihood(ActivationBinaryTree tree,
            List<ParametrizedTemplate> model, ActivationNode conflictResolution)
        {
            double result = 0;
            List<Event> kOfConflictActivations = GetConflict(tree);
            List<BiTemplate> constraints = model
                .Where(x => x.TemplateDescription.TemplateParametersType == TemplateTypes.BiTemplate)
                .SelectMany(x => x.TemplateInstances).Cast<BiTemplate>().ToList();
            foreach (Event resolution in kOfConflictActivations)
            {
                int gama = 0;
                foreach (var constraint in constraints)
                {
                    ActivationBinaryTree tempTree = ActivationTreeBuilder.BuildTree(conflictResolution.Subtrace, constraint);
                    if (GetFulfillment(tempTree).Contains(resolution))
                        gama += 1;
                    else if (GetViolation(tempTree).Contains(resolution))
                        gama += 1;
                }

                result += gama;
            }

            return result / kOfConflictActivations.Count;
        }

        public static TraceEvaluation EvaluateTrace(DeclareModel model, List<Event> trace)
        {
            List<TemplateEvaluation> templateEvaluations = new List<TemplateEvaluation>();
            TraceEvaluation evaluation = new(trace, templateEvaluations);
            
            foreach (var template in model.Constraints)
            {
                if (template.TemplateDescription.TemplateParametersType != TemplateTypes.BiTemplate)
                    continue;
                
                List<ConstraintEvaluation> constraintEvaluations = new List<ConstraintEvaluation>();
                TemplateEvaluation temp = new(constraintEvaluations, template);
                foreach (var constraint in template.TemplateInstances)
                {
                    ActivationBinaryTree tree = ActivationTreeBuilder.BuildTree(trace, (BiTemplate)constraint);
                    List<Event> f = GetFulfillment(tree);
                    List<Event> v = GetViolation(tree);
                    List<Event> c = GetConflict(tree, v, f);
                    List<WrappedEventActivation> eventActivations =
                        GetEventActivationTypes(tree, trace, f, c, v);
                    Healthiness healthiness = new Healthiness(tree, v.Count, f.Count, c.Count);

                    ConstraintEvaluation constraintEvaluation =
                        new ConstraintEvaluation(constraint, healthiness, eventActivations);
                    constraintEvaluations.Add(constraintEvaluation);
                }
                temp.UpdateHealthiness();
                templateEvaluations.Add(temp);
            }
            evaluation.UpdateHealthiness();
            return evaluation;
        }

        private static List<WrappedEventActivation> GetEventActivationTypes(ActivationBinaryTree tree, List<Event> events,
            List<Event> fulfilment, List<Event> conflict, List<Event> violation)
        {
            fulfilment ??= GetFulfillment(tree);
            violation ??= GetViolation(tree);
            conflict ??= GetConflict(tree, violation, fulfilment);
            

            List<WrappedEventActivation> lst = new();
            foreach (var e in events)
            {
                if (fulfilment.Contains(e, new EventEqualityComparer()))
                    lst.Add(new(e, EventActivationType.Fulfilment));
                else if (conflict.Contains(e, new EventEqualityComparer()))
                    lst.Add(new(e, EventActivationType.Conflict));
                else if (violation.Contains(e, new EventEqualityComparer()))
                    lst.Add(new(e, EventActivationType.Violation));
                else
                    lst.Add(new(e, EventActivationType.None));
            }

            return lst;
        }
        
        


    }
}