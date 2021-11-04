using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Models
{
    public record SimpleTemplateEvaluation
    {
        public ParametrisedTemplate Template { get; }

        public List<ITemplate> constraints { get; }

        public Dictionary<ITemplate, Dictionary<Event, EventActivationType>> evals { get; }

        public SimpleTemplateEvaluation(ParametrisedTemplate template)
        {
            Template = template;
            constraints = new();
            evals = new();
        }
    }
}