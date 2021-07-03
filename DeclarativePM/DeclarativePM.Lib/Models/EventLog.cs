using System.Collections.Generic;
using System.Linq;

namespace DeclarativePM.Lib.Models
{
    public class EventLog
    {
        public List<string> headers { get; }
        public List<Event> logs { get; }

        public EventLog(List<Event> logs)
        {
            if (!logs.Any())
            {
                headers = new();
                this.logs = new();
                return;
            }

            headers = Enumerable.Range(0, logs.FirstOrDefault().Count()).Select(i => i.ToString()).ToList();
            this.logs = logs;
        }

        public EventLog(List<Event> logs, List<string> headers)
        {
            this.logs = logs;
            this.headers = headers;
        }

        private List<string> cases;

        public List<string> Cases()
            => cases ?? logs.Select(e => e.CaseId).Distinct().ToList();

        public List<Event> SpecificCase(string @case)
            => logs.Where(e => e.CaseId.Equals(@case)).ToList();

    }
}