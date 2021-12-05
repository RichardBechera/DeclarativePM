using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Chain Response template
    /// Each time A occurs, then B occurs immediately after 
    /// subsequent(A => next(B))
    /// </summary>
    public class ChainResponse: BiTemplate
    {
        public ChainResponse(string logEventA, string logEventB): base(logEventA, logEventB)
        {
        }

        public override LtlExpression GetExpression()
        {
            //subsequent(A => next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
        }
        
        public override bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public override LtlExpression GetVacuityCondition()
        {
            //eventual(A)
            return new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA));
        }
        
        public override string ToString() 
            => $"ChainResponse(\"{LogEventA}\", \"{LogEventB}\")";
    }
}