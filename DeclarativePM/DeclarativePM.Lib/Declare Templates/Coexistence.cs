using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Coexistence template
    /// If B occurs, then A occurs, and vice versa
    /// eventual(A) <=> eventual(B)
    /// </summary>
    public class Coexistence: BiTemplate
    {
        public Coexistence(string logEventA, string logEventB): base(logEventA, logEventB)
        {
        }

        public override LtlExpression GetExpression()
        {
            //eventual(A) <=> eventual(B)
            return new LtlExpression(Operators.Equivalence,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public override bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);

        public override LtlExpression GetVacuityCondition()
        {
            //eventual(A) || eventual(B)
            return new LtlExpression(Operators.Or, 
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public override string ToString() 
            => $"Coexistence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}