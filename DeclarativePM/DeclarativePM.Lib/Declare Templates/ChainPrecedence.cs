using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Chain Precedence template
    /// Each time B occurs, then A occurs immediately before
    /// subsequent(next(B) => A)
    /// </summary>
    public class ChainPrecedence: BiTemplate
    {
        public ChainPrecedence(string logEventA, string logEventB): base(logEventA, logEventB)
        {
        }

        public override LtlExpression GetExpression()
        {
            //subsequent(next(B) => A)
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB)),
                new LtlExpression(LogEventA)));
        }
        
        public override bool IsActivation(Event e)
            => e.Activity.Equals(LogEventB);
        
        public override LtlExpression GetVacuityCondition()
        {
            //eventual(B)
            return new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB));
        }
        
        public override string ToString() 
            => $"ChainPrecedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}