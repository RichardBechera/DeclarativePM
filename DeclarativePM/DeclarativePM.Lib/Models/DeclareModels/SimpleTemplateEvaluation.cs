using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Models.DeclareModels
{
    public record SimpleTemplateEvaluation
    {
        public ParametrizedTemplate Template { get; }

        public List<ITemplate> constraints { get; }

        public Dictionary<ITemplate, Dictionary<Event, EventActivationType>> evals { get; }

        public SimpleTemplateEvaluation(ParametrizedTemplate template)
        {
            Template = template;
            constraints = new();
            evals = new();
        }
    }
}