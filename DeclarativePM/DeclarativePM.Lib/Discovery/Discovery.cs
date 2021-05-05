using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Discovery
{
    public class Discovery
    {
        public void DiscoverModel(IEnumerable<ImportedEventLog> log)
        {
            var templates = GetTemplates();

            var longestCase = log.GroupBy(e => e.CaseId, e => e.CaseId,
                (k, v) => v.Count()).Max();
            
            var neededCombinations = templates
                .Select(t => t.GetMethod("GetAmountOfArguments")?.Invoke(null, null) ?? -1)
                .Cast<int>()
                .Where(a => a >= 0)
                .Distinct();

            var bagOfActivities = log.Select(e => e.Activity).Distinct().ToArray();

            var combinations = neededCombinations.ToDictionary(c => c,
                c => Combinations(c, bagOfActivities)
            );

            var candidates = GenerateCandidateConstraints(templates, combinations, longestCase);
            
            var result = GetMatchingConstraints(log, candidates);
            

            Console.WriteLine();

        }

        public List<ITemplate> GenerateCandidateConstraints(List<Type> templates, Dictionary<int, List<List<string>>> combinations, int longestCase)
        {
            var candidates = new List<ITemplate>();
            
            foreach (var template in templates)
            {
                var arguments = (int) template.GetMethod("GetAmountOfArguments")?.Invoke(null, null)!;

                var constructor = GetTemplateConstructor(arguments, template, out bool noInt);
                
                if (constructor is null || arguments < 0)
                {
                    continue;
                }

                foreach (var combination in combinations[arguments])
                {
                    if (noInt)
                    {
                        var candidate = (ITemplate) constructor.Invoke(combination.ToArray());
                        candidates.Add(candidate);
                        continue;
                    }

                    for (int i = 1; i < longestCase; i++)
                    {
                        var par = new List<object>() {i};
                        par.AddRange(combination);
                        var input = par.ToArray();
                        var candidate = (ITemplate) constructor.Invoke(input);
                        candidates.Add(candidate);
                    }
                    
                }
                
            }

            return candidates;
        }

        public List<ITemplate> GetMatchingConstraints(IEnumerable<ImportedEventLog> log, List<ITemplate> candidates)
        {
            var instances = log.GroupBy(e => e.CaseId);
            var result = new List<ITemplate>();
            foreach (var candidate in candidates)
            {
                bool cont = false;
                var expr = candidate.GetExpression();
                foreach (var instance in instances)
                {
                    if (EvaluateExpression(instance.ToList(), expr, 0)) continue;
                    cont = true;
                }
                if (cont)
                    continue;
                result.Add(candidate);
            }

            return result;
        }

        private List<Type> GetTemplates() => Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(t => t.Namespace != null
                         && t.IsValueType 
                         && t.Namespace.Equals(@"DeclarativePM.Lib.Declare_Templates")
                         && t.IsAssignableTo(typeof(ITemplate)))
             .ToList();

        private ConstructorInfo GetTemplateConstructor(int arguments, Type template, out bool noInt)
        {
            var methodInfo = template.GetMethod("GetConstructorOptions");
            var options = (Type[]) methodInfo?.Invoke(null, null);

            noInt = !template.IsAssignableTo(typeof(IExistenceTemplate));

            var constructor = template.GetConstructor(options);

            return constructor;
        }

        List<List<T>> Combinations<T>(int rest, T[] bag)
        {
            var res = new List<List<T>>();
            List<List<T>> recursive = null;
            
            if (rest != 1)
                recursive = Combinations(rest - 1, bag);

            foreach (var t in bag)
            {
                if (rest == 1)
                {
                    res.Add(new List<T> {t} );
                    continue;
                }
                res.AddRange(recursive?
                    .Select(c => new List<T>(c) {t})!);
            }
            return res;
        }

        bool EvaluateExpression(List<ImportedEventLog> events, LtlExpression expression, int position)
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
    }
}