using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public class TraceEvaluation
    {
        public List<Event> Trace { get; }
        public List<TemplateEvaluation> TemplateEvaluations { get; }

        public TraceEvaluation(List<Event> trace, List<TemplateEvaluation> templateEvaluations)
        {
            Trace = trace;
            TemplateEvaluations = templateEvaluations;
        }
    }
}