using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public record ConstraintEvaluation
    {
        public ITemplate Constraint { get; }
        public Healthiness Healthiness { get; }

        public List<WrappedEventActivation> Activations { get; }

        public ConstraintEvaluation(ITemplate constraint, Healthiness healthiness, List<WrappedEventActivation> activations)
        {
            Constraint = constraint;
            Healthiness = healthiness;
            Activations = activations;
        }
    }
}