using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.UI.Data
{
    public class TraceDTO
    {
        public List<Event> Events { get; set; }
        
        public string Case { get; set; }

        public TraceDTO(List<Event> events)
        {
            Events = events;
            Case = events.Count > 0 ? events.First().CaseId : "NO EVENTS INCLUDED!";
        }
    }
}