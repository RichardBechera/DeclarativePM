using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Absence : IExistenceTemplate
    {
        public int Occurances;
        public string LogEvent;
        
        public Absence(int occurances, string logEvent)
        {
            LogEvent = logEvent;
            Occurances = occurances;
        }

        public static int GetAmountOfArguments() => 1;

        public static Type[] GetConstructorOptions() => new[] {typeof(int), typeof(string)};

        public LtlExpression GetExpression()
        {
            return new LtlExpression(Operators.Not, new Existence(Occurances, LogEvent).GetExpression());
        }

        public override string ToString() 
            => $"Absence({Occurances}, \"{LogEvent}\")";

        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurances;
    }
}