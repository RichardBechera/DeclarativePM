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
    public struct ChainResponse: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public ChainResponse(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A => next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public LtlExpression GetVacuityCondition()
        {
            //eventual(A)
            return new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA));
        }
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"ChainResponse(\"{LogEventA}\", \"{LogEventB}\")";
    }
}