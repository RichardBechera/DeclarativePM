using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct AlternateSuccession: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
        public AlternateSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //alternateResponse(A, B) && AlternatePrecedence(A, B)
            return new LtlExpression(Operators.And, new AlternateResponse(LogEventA, LogEventB).GetExpression(),
                new AlternatePrecedence(LogEventA, LogEventB).GetExpression());
        }
    }
}