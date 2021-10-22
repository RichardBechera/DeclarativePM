using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeclarativePM.Lib.Models
{
    public class EventLog
    {
        public List<string> Headers { get; }
        public List<Event> Logs { get; }

        public string Name { get; set; }

        public EventLog(List<Event> logs, string name = null)
        {
            if (!logs.Any())
            {
                Headers = new();
                Logs = new();
                return;
            }

            Headers = Enumerable.Range(0, logs.FirstOrDefault().Count()).Select(i => i.ToString()).ToList();
            Logs = logs;
            Name = name ?? GetDefaultName();
        }

        public EventLog(List<Event> logs, List<string> headers, string name = null)
        {
            Logs = logs;
            Headers = headers;
            Name = name ?? GetDefaultName();
        }

        public List<string> Cases()
            => Logs.Select(e => e.CaseId).Distinct().ToList();

        public List<Event> SpecificCase(string @case)
            => Logs.Where(e => e.CaseId.Equals(@case)).ToList();

        private string GetDefaultName()
            => "DEFAULT LOG NAME";
        

    }
}