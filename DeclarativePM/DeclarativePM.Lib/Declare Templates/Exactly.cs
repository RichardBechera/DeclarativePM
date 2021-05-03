using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Exactly: ITemplate
    {
        public int Occurances;
        public string LogEvent;
        public const int NumberOfArguments = 1;
        
        public Exactly(int occurances, string logEvent)
        {
            Occurances = occurances;
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            //existence(n, A) && absence(n + 1, A)
            return new LtlExpression(Operators.And, new Existence(Occurances, LogEvent).GetExpression(),
                new Absence(Occurances + 1, LogEvent).GetExpression());
        }
    }
}