using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.LogModels;
using Newtonsoft.Json;

namespace DeclarativePM.Lib.Models.DeclareModels
{
    /// <summary>
    /// Declare model containing all the templates and corresponding instances (constraints)
    /// </summary>
    public class DeclareModel
    {
        public string Name { get; set; }
        public List<ParametrizedTemplate> Constraints { get; set; }
        
        [JsonIgnore]
        public EventLog Log { get; }

        public DeclareModel(string name, List<ParametrizedTemplate> constraints, EventLog log)
        {
            Name = name;
            Constraints = constraints;
            Log = log;
        }
        
        [JsonConstructor]
        public DeclareModel(string name, List<ParametrizedTemplate> constraints)
        {
            Name = name;
            Constraints = constraints;
            Log = new (new());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>all unique activities in the model</returns>
        /// <exception cref="ArgumentOutOfRangeException">Log does not exist and there is template of other type than
        /// those defined.</exception>
        public List<string> GetAllActivities()
        {
            if (Log is not null && Log.Logs.Count != 0)
                return Log.GetAllActivities();
            HashSet<string> activities = new();
            foreach (var template in Constraints)
            {
                foreach (var instance in template.TemplateInstances)
                {
                    switch(template.TemplateDescription.TemplateParametersType)
                    {
                        case TemplateTypes.Existence:
                            ExistenceTemplate temp1 = (ExistenceTemplate) instance;
                            activities.Add(temp1.GetEvent());
                            break;
                        case TemplateTypes.BiTemplate:
                            BiTemplate temp2 = (BiTemplate) instance;
                            activities.Add(temp2.GetEventA());
                            activities.Add(temp2.GetEventB());
                            break;
                        case TemplateTypes.UniTemplate:
                            UniTemplate temp3 = (UniTemplate) instance;
                            activities.Add(temp3.GetEventA());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            return activities.ToList();
        }
    }
}