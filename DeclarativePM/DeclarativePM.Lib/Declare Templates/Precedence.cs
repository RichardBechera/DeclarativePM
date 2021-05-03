using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Precedence: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
        public Precedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //(!B U A) || subsequent(!B)
            return new LtlExpression(Operators.Or, new LtlExpression(Operators.Least,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB)),
                    new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Subsequent,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB))));
        }
    }
}