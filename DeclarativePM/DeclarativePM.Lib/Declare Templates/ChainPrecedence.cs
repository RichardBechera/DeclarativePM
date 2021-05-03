using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct ChainPrecedence: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
        public ChainPrecedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(next(B) => A)
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB)),
                new LtlExpression(LogEventA)));
        }
    }
}