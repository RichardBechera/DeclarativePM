using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Response template
    /// If A occurs, then B occurs after A
    /// subsequent(A => eventual(B))
    /// </summary>
    public struct Response: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public Response(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A => eventual(B))
            return new LtlExpression(Operators.Subsequent,
                new LtlExpression(Operators.Imply, new LtlExpression(LogEventA),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"Response(\"{LogEventA}\", \"{LogEventB}\")";
    }
}