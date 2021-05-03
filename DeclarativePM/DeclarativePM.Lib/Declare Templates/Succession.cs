using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Succession: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
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
    }
}