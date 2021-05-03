using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Init: ITemplate
    {
        public string LogEvent;
        public const int NumberOfArguments = 1;
        
        public Init(string logEvent)
        {
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            //A
            return new LtlExpression(LogEvent);
        }
    }
}