using System.Collections.Generic;
using System.Linq;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public class TraceEvaluation
    {
        public List<Event> Trace { get; }
        public List<TemplateEvaluation> TemplateEvaluations { get; }
        public Healthiness Healthiness { get; private set; }

        public TraceEvaluation(List<Event> trace, List<TemplateEvaluation> templateEvaluations)
        {
            Trace = trace;
            TemplateEvaluations = templateEvaluations;
            Healthiness = new Healthiness(templateEvaluations.Select(e => e.Healthiness).ToList());
        }
        
        public void UpdateHealthiness()
        {
            Healthiness = new Healthiness(TemplateEvaluations.Select(e => e.Healthiness).ToList());
        }
    }
}