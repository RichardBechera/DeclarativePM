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
                .Select(t => ((int) (t.Template.GetMethod("GetAmountOfArguments")?
                    .Invoke(null, null) ?? -1), t))
                .Where(a => a.Item1 >= 0)
                .ToList();
                
            
            var instances = importedEventLogs.GroupBy(e => e.CaseId).ToList();

            var ordering = OrderedEvents(instances);
            
            var candidates = GenerateCandidates(argOnTemplates, ordering, longestCase, isGeneralPoX, poe);

            var instancesAsLists = instances.Select(x => x.ToList());
            
            return GetMatchingConstraints(instancesAsLists, candidates, poi, isGeneralPoX);
        }

        /// <summary>
        /// Generates all the candidates for resulting DECLARE model.
        /// </summary>
        /// <param name="argOnTemplates">Tuples where first argument is amount of arguments template constructor takes
        /// and second one is corresponding parametrised template.</param>
        /// <param name="ordering">List of unique ordered Event Activities from the most frequent
        /// to the least frequent and their frequencies.</param>
        /// <param name="longestCase">Longest case in an Event log.</param>
        /// <param name="isGeneralPoX">Whether we use POE for whole Event log or
        /// POE corresponding to the given template.</param>
        /// <param name="poe">Percentage of most frequent events we consider.</param>
        /// <returns>List of all candidates for DECLARE model.</returns>
        private List<ParametrisedTemplate> GenerateCandidates(List<(int, ParametrisedTemplate t)> argOnTemplates,
            List<(string, int)> ordering, int longestCase, bool isGeneralPoX, decimal poe = 100)
        {
            var candidates = new List<ParametrisedTemplate>();
            string[] bagOfEvents;
            
            if (!isGeneralPoX)
            {
                //TODO performance tweaks: where poe and args same => cash combinations
                foreach (var combination in argOnTemplates)
                {
                    bagOfEvents = ReduceEvents(ordering, combination.t.Poe).ToArray();
                    var combo = UtilMethods.Combinations(combination.Item1, bagOfEvents, false);
                    InnerCandidateGeneration(combination.t, candidates, combo, longestCase, combination.Item1);
                }
                return candidates;
            }
            
            var neededCombinations = argOnTemplates
                .GroupBy(k => k.Item1, v => v.t);
            bagOfEvents = ReduceEvents(ordering, poe).ToArray();
            foreach (var combination in neededCombinations)
            {
                var combo = UtilMethods.Combinations(combination.Key, bagOfEvents, false);
                foreach (var template in combination)
                {
                    InnerCandidateGeneration(template, candidates, combo, longestCase, combination.Key);
                }
            }

            return candidates;
        }

        /// <summary>
        /// Generates all the candidates for resulting DECLARE model.
        /// </summary>
        /// <param name="template">Template for which we are generating all possible candidates.</param>
        /// <param name="candidates"> List of candidates which to fill with generated candidates.</param>
        /// <param name="combinations">All possible combinations of event of length of amount of arguments
        /// which template takes in constructor.</param>
        /// <param name="longestCase">Longest case in an Event log.</param>
        /// <param name="arguments">Amount of arguments template constructor takes.</param>
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
                    var candidate = new ParametrisedTemplate(template, (ITemplate) constructor.Invoke(combination.ToArray()));
                    candidates.Add(candidate);
                    continue;
                }

                for (var i = 1; i < longestCase; i++)
                {
                    var par = new List<object>() {i};
                    par.AddRange(combination);
                    var input = par.ToArray();
                    var candidate = new ParametrisedTemplate(template, (ITemplate) constructor.Invoke(input));
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
            List<ParametrisedTemplate> candidates, decimal poi, bool usePoi = true)
        {
            UtilMethods.CutIntoRange(ref poi, 1);
            int treshold = (int)decimal.Round(candidates.Count() * (100 - poi) / 100);
            var enumerable = instances.ToList();
            var count = candidates.Count;

            var r = candidates.AsParallel()
                .Where(c => CheckConstraint(c, treshold, enumerable, count, usePoi));

            return r.Select(t => t.TemplateInstance).ToList();
        }

        /// <summary>
        /// Checks whether given constraint is satisfied in a given event log.
        /// </summary>
        /// <param name="candidate">DECLARE constraint.</param>
        /// <param name="treshold">Amount of instances on which checking can fail.</param>
        /// <param name="instances">Instances from event log, each represents a unique case.</param>
        /// <param name="allCount">Amount of all instances in an Event Log.</param>
        /// <param name="usePoi">Whether to compute new treshold according to poi of templates.</param>
        /// <returns>True if constraint hold, false else.</returns>
        private bool CheckConstraint(ParametrisedTemplate candidate, int treshold, List<List< 
            ImportedEventLog>> instances, int allCount, bool usePoi)
        {
            bool cont = false;
            int notHolds = 0;
            var expr = candidate.TemplateInstance?.GetExpression();
            if (expr is null)
                return false;
            treshold = !usePoi ? (int)decimal.Round(allCount * (100 - candidate.Poi) / 100) : treshold;
            foreach (var instance in instances)
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

        /// <summary>
        /// Function orders unique events from an event log from the most frequent to the least frequent one.
        /// </summary>
        /// <param name="instances"> Grouping of unique events and all their instances.</param>
        /// <returns>List of unique ordered Event Activities from the most frequent
        /// to the least frequent and their frequencies.</returns>
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