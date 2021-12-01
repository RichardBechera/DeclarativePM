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
    public struct Succession: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public Succession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //response(A, B) && precedence(A, B)
            return new LtlExpression(Operators.And, new Response(LogEventA, LogEventB).GetExpression(),
                new Precedence(LogEventA, LogEventB).GetExpression());
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"Succession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}