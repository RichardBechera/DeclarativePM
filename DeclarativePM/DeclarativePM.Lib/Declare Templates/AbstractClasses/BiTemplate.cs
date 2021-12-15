using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;

namespace DeclarativePM.Lib.Declare_Templates.AbstractClasses
{
    /// <summary>
    /// Represents templates which take two events as parameters.
    /// Those are relational and not relational templates.
    /// </summary>
    public abstract class BiTemplate : ITemplate, IVacuityDetection
    {
        public readonly string LogEventA;
        public readonly string LogEventB;

        protected BiTemplate(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        public abstract bool IsActivation(Event e);

        public abstract LtlExpression GetVacuityCondition();

        public LtlExpression GetWitnessExpression() =>
            new LtlExpression(Operators.And, GetExpression(), GetVacuityCondition());

        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public abstract LtlExpression GetExpression();
        
    }
}