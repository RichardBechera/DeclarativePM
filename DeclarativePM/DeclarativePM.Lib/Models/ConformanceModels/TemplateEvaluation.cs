using System.Collections.Generic;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public record TemplateEvaluation
    {
        public List<ConstraintEvaluation> ConstraintEvaluations { get; }
        public ParametrizedTemplate Template { get; }

        public TemplateEvaluation(List<ConstraintEvaluation> constraintEvaluations, ParametrizedTemplate template)
        {
            ConstraintEvaluations = constraintEvaluations;
            Template = template;
        }
    }
}