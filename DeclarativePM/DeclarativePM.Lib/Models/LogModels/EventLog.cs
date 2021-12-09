using System.Collections.Generic;
using System.Linq;

namespace DeclarativePM.Lib.Models.LogModels
{
    /// <summary>
    /// Represents an event log
    /// </summary>
    public class EventLog
    {
        public List<string> Headers { get; }
        public List<Event> Logs { get; }

        public string Name { get; set; }

        public EventLog(List<Event> logs, string name = null)
        {
            Name = name ?? DefaultName;
            if (!logs.Any())
            {
                Headers = new();
                Logs = new();
                return;
            }

            Headers = Enumerable.Range(0, logs.FirstOrDefault().Count()).Select(i => i.ToString()).ToList();
            Logs = logs;
        }

        public EventLog(List<Event> logs, List<string> headers, string name = null)
        {
            Logs = logs;
            Headers = headers;
            Name = name ?? DefaultName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>All unique cases in the log</returns>
        public List<string> Cases()
            => Logs.Select(e => e.CaseId).Distinct().ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="case">search case</param>
        /// <returns>the trace under specific case</returns>
        public List<Event> SpecificCase(string @case)
            => Logs.Where(e => e.CaseId.Equals(@case)).ToList();
        
        private string DefaultName
            => "DEFAULT LOG NAME";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>All unique activities in the log</returns>
        public List<string> GetAllActivities()
            => Logs.Select(x => x.Activity).Distinct().ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of all traces in the log</returns>
        public List<List<Event>> GetAllTraces()
            => Logs.GroupBy(x => x.CaseId, x => x, (_, events) => events.ToList()).ToList();


    }
}