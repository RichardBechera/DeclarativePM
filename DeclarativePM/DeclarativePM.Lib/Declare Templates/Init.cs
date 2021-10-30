using System;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Init: IUniTemplate
    {
        public string LogEvent;
        
        public Init(string logEvent)
        {
            LogEvent = logEvent;
        }
        
        public static int GetAmountOfArguments() => 1;

        public static Type[] GetConstructorOptions() => new[] {typeof(string)};

        public LtlExpression GetExpression()
        {
            //A
            return new LtlExpression(LogEvent);
        }

        public string GetEventA()
            => LogEvent;

        public override string ToString() 
            => $"Init(\"{LogEvent}\")";
    }
}