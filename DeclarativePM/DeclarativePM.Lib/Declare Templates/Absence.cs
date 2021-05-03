using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Absence : ITemplate
    {
        public int Occurances;
        public string LogEvent;
        public const int NumberOfArguments = 1;
        
        public Absence(int occurances, string logEvent)
        {
            LogEvent = logEvent;
            Occurances = occurances;
        }

        public LtlExpression GetExpression()
        {
            return new LtlExpression(Operators.Not, new Existence(Occurances, LogEvent).GetExpression());
        }
    }
}