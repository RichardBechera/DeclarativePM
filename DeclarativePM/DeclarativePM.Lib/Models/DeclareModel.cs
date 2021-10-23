using System.Collections.Generic;

namespace DeclarativePM.Lib.Models
{
    public class DeclareModel
    {
        public string Name { get; set; }
        public List<ParametrisedTemplate> Constraints { get; set; }
        public EventLog Log { get; set; }

        public DeclareModel(string name, List<ParametrisedTemplate> constraints, EventLog log)
        {
            Name = name;
            Constraints = constraints;
            Log = log;
        }
    }
}