using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Not Coexistence template
    /// A and B never occur together 
    /// !(eventual(A) && eventual(B))
    /// </summary>
    public struct NotCoexistence: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public NotCoexistence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //!(eventual(A) && eventual(B))
            return new LtlExpression(Operators.Not, new LtlExpression(Operators.And,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public LtlExpression GetVacuityCondition()
        {
            //eventual(A) || eventual(B)
            return new LtlExpression(Operators.Or, 
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"NotCoexistence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}