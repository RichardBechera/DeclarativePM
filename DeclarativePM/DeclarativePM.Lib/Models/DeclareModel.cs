using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;

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
        
        public DeclareModel(string name, List<ParametrisedTemplate> constraints)
        {
            Name = name;
            Constraints = constraints;
        }

        public List<string> GetAllActivities()
        {
            if (Log is not null)
                return Log.GetAllActivities();
            HashSet<string> activities = new();
            foreach (var template in Constraints)
            {
                foreach (var instance in template.TemplateInstances)
                {
                    switch(template.TemplateType)
                    {
                        case TemplateTypes.Existence:
                            IExistenceTemplate temp1 = (IExistenceTemplate) instance;
                            activities.Add(temp1.GetEvent());
                            break;
                        case TemplateTypes.BiTemplate:
                            IBiTemplate temp2 = (IBiTemplate) instance;
                            activities.Add(temp2.GetEventA());
                            activities.Add(temp2.GetEventB());
                            break;
                        case TemplateTypes.UniTemplate:
                            IUniTemplate temp3 = (IUniTemplate) instance;
                            activities.Add(temp3.GetEventA());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    };
                }
            }
            return activities.ToList();
        }
    }
}