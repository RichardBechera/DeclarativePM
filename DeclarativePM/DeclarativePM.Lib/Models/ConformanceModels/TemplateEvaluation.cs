using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public record TemplateEvaluation
    {
        public List<ConstraintEvaluation> ConstraintEvaluations { get; }
        public ParametrizedTemplate Template { get; }

        public Healthiness Healthiness { get; private set; }

        public TemplateEvaluation(List<ConstraintEvaluation> constraintEvaluations, ParametrizedTemplate template)
        {
            ConstraintEvaluations = constraintEvaluations;
            Template = template;
        }

        public void UpdateHealthiness()
        {
            Healthiness = new Healthiness(ConstraintEvaluations.Select(e => e.Healthiness).ToList());
        }
    }
}