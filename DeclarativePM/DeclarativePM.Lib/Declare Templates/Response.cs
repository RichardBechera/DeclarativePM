using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Response: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
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
    }
}