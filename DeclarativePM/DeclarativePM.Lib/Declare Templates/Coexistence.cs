using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Coexistence: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
        public Coexistence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //eventual(A) <=> eventual(B)
            return new LtlExpression(Operators.Equivalence,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
    }
}