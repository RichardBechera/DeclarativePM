using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct ChainSuccession: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
        public ChainSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A <=> next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Equivalence,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
        }
    }
}