using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Succession template
    /// A occurs if and only if B occurs after A
    /// response(A, B) && precedence(A, B)
    /// </summary>
    public class Succession: BiTemplate
    {
        public Succession(string logEventA, string logEventB): base(logEventA, logEventB)
        {
        }

        public override LtlExpression GetExpression()
        {
            //response(A, B) && precedence(A, B)
            return new LtlExpression(Operators.And, new Response(LogEventA, LogEventB).GetExpression(),
                new Precedence(LogEventA, LogEventB).GetExpression());
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
            => $"Succession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}