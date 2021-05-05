using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Exactly: IExistenceTemplate
    {
        public int Occurances;
        public string LogEvent;
        
        public Exactly(int occurances, string logEvent)
        {
            Occurances = occurances;
            LogEvent = logEvent;
        }
        
        public static int GetAmountOfArguments() => 1;

        public static Type[] GetConstructorOptions() => new[] {typeof(int), typeof(string)};

        public LtlExpression GetExpression()
        {
            //existence(n, A) && absence(n + 1, A)
            return new LtlExpression(Operators.And, new Existence(Occurances, LogEvent).GetExpression(),
                new Absence(Occurances + 1, LogEvent).GetExpression());
        }
    }
}