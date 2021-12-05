using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Chain Succession template
    /// A and B occur if and only if the latter immediately follows the former
    /// subsequent(A <=> next(B))
    /// </summary>
    public class ChainSuccession: BiTemplate
    {
        public ChainSuccession(string logEventA, string logEventB): base(logEventA, logEventB)
        {
        }

        public override LtlExpression GetExpression()
        {
            //subsequent(A <=> next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Equivalence,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
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
            => $"ChainSuccession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}