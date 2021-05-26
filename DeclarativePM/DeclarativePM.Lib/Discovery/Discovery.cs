using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Discovery
{
    public class Discovery
    {
        /// <summary>
        /// Method discovers DECLARE model on top of an event log.
        /// </summary>
        /// <param name="log">Event log.</param>
        /// <param name="poe">Percentage of events. 100 for discovery on every event in an event log.
        /// For n where 0 < =n < 100, =n% of most frequent events in the log.</param>
        /// <param name="poi">Percentage of instances. Defines percentage on how many instances does
        /// template has to hold to be considered in the resulting DECLARE model.</param>
        /// <returns>DECLARE model representing an event log.</returns>
        public List<ITemplate> DiscoverModel(IEnumerable<ImportedEventLog> log, decimal poe = 100, decimal poi = 100)
        {
            return DiscoverModel(log, GetTemplates(), poe, poi);
        }
        
        /// <summary>
        /// Method discovers DECLARE model on top of an event log.
        /// </summary>
        /// <param name="log">Event log.</param>
        /// <param name="templates">List of desired templates which will be in the resulting DECLARE model.</param>
        /// <param name="poe">Percentage of events. 100 for discovery on every event in an event log.
        /// For n where 0 < =n < 100, =n% of most frequent events in the log.</param>
        /// <param name="poi">Percentage of instances. Defines percentage on how many instances does
        /// template has to hold to be considered in the resulting DECLARE model.</param>
        /// <returns>DECLARE model representing an event log.</returns>
        public List<ITemplate> DiscoverModel(IEnumerable<ImportedEventLog> log, List<Type> templates, decimal poe = 100, decimal poi = 100)
        {
            var temp = templates
                .Where(t => t.IsValueType && t.IsAssignableTo(typeof(ITemplate)))
                .Select(t => new ParametrisedTemplate(t))
                .ToList();
            return DiscoverModel(log, temp, true, poe, poi);
        }

        //TODO templates
        /// <summary>
        /// Method discovers DECLARE model on top of an event log.
        /// </summary>
        /// <param name="log">Event log.</param>
        /// <param name="templates">List of desired templates which will be in the resulting DECLARE model.</param>
        /// <returns>DECLARE model representing an event log.</returns>
        public List<ITemplate> DiscoverModel(IEnumerable<ImportedEventLog> log, List<ParametrisedTemplate> templates)
        {
            var temp = templates
                .Where(t => t.Template.IsValueType && t.Template.IsAssignableTo(typeof(ITemplate)))
                .ToList();
            return DiscoverModel(log, temp, false);
        }

        /// <summary>
        /// Method discovers DECLARE model on top of an event log.
        /// </summary>
        /// <param name="log">Event log.</param>
        /// <param name="templates">List of desired templates which will be in the resulting DECLARE model.</param>
        /// <param name="isGeneralPoX">States whether POE and POI are used in general on on each template the same or
        /// whether each template has it's own poe and poi</param>
        /// <param name="poe">Percentage of events. 100 for discovery on every event in an event log.
        /// For n where 0 < =n < 100, =n% of most frequent events in the log. </param>
        /// <param name="poi">Percentage of instances. Defines percentage on how many instances does
        /// template has to hold to be considered in the resulting DECLARE model.</param>
        /// <returns>DECLARE model representing an event log.</returns>
        private List<ITemplate> DiscoverModel(IEnumerable<ImportedEventLog> log, List<ParametrisedTemplate> templates, 
            bool isGeneralPoX, decimal poe = 100, decimal poi = 100)
        {
            var importedEventLogs = log as ImportedEventLog[] ?? log.ToArray();
            var longestCase = importedEventLogs.GroupBy(e => e.CaseId, e => e.CaseId,
                (_, v) => v.Count()).Max();
            
            //TODO reflexion
            var argOnTemplates = templates
                .Select(t => new {arguments = (int) (t.Template.GetMethod("GetAmountOfArguments")?
                    .Invoke(null, null) ?? -1), t})
                .ToList();
                
            
            var instances = importedEventLogs.GroupBy(e => e.CaseId).ToList();

            var ordering = OrderedEvents(instances);

            Dictionary<int, List<List<string>>> combinations;
            List<ParametrisedTemplate> candidates;
            
            if (isGeneralPoX)
            {
                var neededCombinations = argOnTemplates
                    .Select(v => v.arguments)
                    .Where(a => a >= 0)
                    .Distinct();
                var bagOfEvents = ReduceEvents(ordering, poe).ToArray();
                combinations = neededCombinations.ToDictionary(c => c,
                    c => UtilMethods.Combinations(c, bagOfEvents, false));
                candidates = GenerateCandidateConstraints(templates, combinations, longestCase);
            }
            else
            {
                //TODO performance tweaks: where poi, poe and args same => cash combinations
                candidates = new List<ParametrisedTemplate>();
                var neededCombinations = argOnTemplates
                    .Select(v => new {args = v.arguments, v.t})
                    .Where(a => a.args >= 0) //in case, shouldn't happen
                    .ToList();
                foreach (var combination in neededCombinations)
                {
                    var bagOfEvents = ReduceEvents(ordering, combination.t.Poe).ToArray();
                    var combo = UtilMethods.Combinations(combination.args, bagOfEvents, false);
                    candidates.AddRange(GenerateCandidateConstraints(combination.t, combo, longestCase, combination.args));
                }
            }
            
            var instancesAsLists = instances.Select(x => x.ToList());
            
            return GetMatchingConstraints(instancesAsLists, candidates, poi);
        }

        /// <summary>
        /// Generates list of every single possible template on top of given distinct events.
        /// </summary>
        /// <param name="templates">List of ITemplate types which are to be generated.</param>
        /// <param name="combinations">Dictionary storing list of all possible argument combinations
        /// where key defines amount of arguments, resp. length of combination,
        /// i.e. elements in one combination.</param>
        /// <param name="longestCase">Length of longest case in an event log on which candidates will be checked.</param>
        /// <returns>List of possible templates, candidates.</returns>
        private List<ParametrisedTemplate> GenerateCandidateConstraints(List<ParametrisedTemplate> templates, Dictionary<int, List<List<string>>> combinations, int longestCase)
        {
            var candidates = new List<ParametrisedTemplate>();
            
            foreach (var template in templates)
            {
                //TODO reflexion => call static method somehow else?
                var arguments = (int) template.Template.GetMethod("GetAmountOfArguments")?.Invoke(null, null)!;

                InnerCandidateGeneration(template, candidates, combinations[arguments], longestCase, arguments);
            }
            return candidates;
        }

        private List<ParametrisedTemplate> GenerateCandidateConstraints(ParametrisedTemplate template,
            List<List<string>> combinations, int longestCase, int arguments)
        {
            var candidates = new List<ParametrisedTemplate>();

            InnerCandidateGeneration(template, candidates, combinations, longestCase, arguments);

            return candidates;
        }

        private void InnerCandidateGeneration(ParametrisedTemplate template, List<ParametrisedTemplate> candidates, List<List<string>> combinations, 
            int longestCase, int arguments)
        {
            var constructor = GetTemplateConstructor(template.Template, out bool noInt);
                
            if (constructor is null || arguments < 0)
            {
                return;
            }

            //TODO reflexion
            foreach (var combination in combinations)
            {
                if (noInt)
                {
                    var candidate = new ParametrisedTemplate(template.Template, 
                        (ITemplate) constructor.Invoke(combination.ToArray()), template.Poe, template.Poi);
                    candidates.Add(candidate);
                    continue;
                }

                for (var i = 1; i < longestCase; i++)
                {
                    var par = new List<object>() {i};
                    par.AddRange(combination);
                    var input = par.ToArray();
                    var candidate = new ParametrisedTemplate(template.Template, 
                        (ITemplate) constructor.Invoke(input), template.Poe, template.Poi);
                    candidates.Add(candidate);
                }
            }

        }

        /// <summary>
        /// Reduces list of templates to only templates which hold in an event log for certain percentage of instances.
        /// </summary>
        /// <param name="instances">Grouping of events under different cases on which the check is performed.</param>
        /// <param name="candidates">Candidate templates which are to be checked and reduced.</param>
        /// <param name="poi">Percentage of instances at which candidate needs to be held in order not to be reduced.
        /// 100 in order for candidate to hold in every case, 50 in order to hold in at least 50% of cases.</param>
        /// <returns>List of reduced templates which hold in an event log.</returns>
        private List<ITemplate> GetMatchingConstraints(IEnumerable<List<ImportedEventLog>> instances,
            List<ParametrisedTemplate> candidates, decimal poi)
        {
            UtilMethods.CutIntoRange(ref poi, 1);
            int treshold = (int)decimal.Round(candidates.Count() * (100 - poi) / 100);
            var enumerable = instances.ToList();
            var count = candidates.Count;

            var r = candidates.AsParallel()
                .Where(c => CheckConstraint(c, treshold, enumerable, count, poi));
            
            /*foreach (var candidate in candidates)
            {
                
                result.Add(candidate.TemplateInstance);
            }*/

            return r.Select(t => t.TemplateInstance).ToList();
        }

        private bool CheckConstraint(ParametrisedTemplate candidate, int treshold, List<List< 
            ImportedEventLog>> enumerable, int allCount, decimal poi)
        {
            bool cont = false;
            int notHolds = 0;
            var expr = candidate.TemplateInstance?.GetExpression();
            if (expr is null)
                return false;
            treshold = poi == -1 ? (int)decimal.Round(allCount * (100 - candidate.Poi) / 100) : treshold;
            foreach (var instance in enumerable)
            {
                if (EvaluateExpression(instance, expr)) continue;
                notHolds++;
                if (notHolds < treshold) continue;
                cont = true;
                break;
            }

            return !cont;
        }

        /// <summary>
        /// Method gets every Type from DeclarativePM.Lib.Declare_Templates namespace
        /// which implements ITemplate interface.
        /// </summary>
        /// <returns>List of types implementing ITemplate.</returns>
        private List<Type> GetTemplates() => Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(t => t.Namespace != null
                         && t.IsValueType 
                         && t.Namespace.Equals(@"DeclarativePM.Lib.Declare_Templates")
                         && t.IsAssignableTo(typeof(ITemplate)))
             .ToList();

        /// <summary>
        /// Gets the constructor of types implementing ITemplate namespace
        /// </summary>
        /// <param name="template">Type of template of which we need the constructor.</param>
        /// <param name="noInt">Out parameter whether first argument to constructor is integer,
        /// resp. whether template implements IExistenceTemplate</param>
        /// <returns>ConstructorInfo of template implementing ITemplate.</returns>
        private ConstructorInfo GetTemplateConstructor(Type template, out bool noInt)
        {
            //TODO refelxion
            var methodInfo = template.GetMethod("GetConstructorOptions");
            var options = (Type[]) methodInfo?.Invoke(null, null);

            noInt = !template.IsAssignableTo(typeof(IExistenceTemplate));

            var constructor = options is null ? null : template.GetConstructor(options);

            return constructor;
        }

        /// <summary>
        /// Reduces amount of events based on given percentage. For 100 amount of events stays unchanged.
        /// For 50, 50% of least frequent events are thrown away.
        /// </summary>
        /// <param name="ordering">Ordered cases from which we want to reduce, tuple as distinct events
        /// and their counts.</param>
        /// <param name="poe">Percentage of events.</param>
        /// <returns>List of reduced events, resp. their names.</returns>
        private List<string> ReduceEvents(List<(string, int)> ordering, decimal poe)
        {
            var count = ordering.Count();
            var endingElements = (int)decimal.Round((count * poe) / 100);
            return ordering.Take(endingElements).Select(v => v.Item1).ToList();
        }

        private List<(string, int)> OrderedEvents(IEnumerable<IGrouping<string, ImportedEventLog>> instances)
        {
            return instances.SelectMany(g => g.Select(v => v.Activity))
                .GroupBy(v => v, v => v, (s, enumerable) => (s, enumerable.Count()))
                .OrderByDescending(v => v.Item2)
                .ToList();
        }



        /// <summary>
        /// Evaluates whether expression holds for the case.
        /// </summary>
        /// <param name="events">Sorted list of event in a case.</param>
        /// <param name="expression">Expression to be evaluated.</param>
        /// <param name="position">Starting position from which we start the evaluation.</param>
        /// <returns>Bool whether expression holds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Expression contains undefined operator.</exception>
        bool EvaluateExpression(List<ImportedEventLog> events, LtlExpression expression, int position = 0)
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