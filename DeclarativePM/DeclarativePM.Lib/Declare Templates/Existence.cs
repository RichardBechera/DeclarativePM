using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Existence: IExistenceTemplate
    {
        public int Occurances;
        public string LogEvent;
        
        public Existence(int occurances, string logEvent)
        {
            //what if 0 passed?
            Occurances = occurances;
            LogEvent = logEvent;
        }
        
        public static int GetAmountOfArguments() => 1;

        public static Type[] GetConstructorOptions() => new[] {typeof(int), typeof(string)};

        public LtlExpression GetExpression()
        {
            if (Occurances == 1)
            {
                return new LtlExpression(Operators.Eventual, new LtlExpression(LogEvent));
            }
            //Eventual(A && Next(Existence(n-1, A)))
            return new LtlExpression(Operators.Eventual,
                new LtlExpression(Operators.And, 
                    new LtlExpression(LogEvent),
                    new LtlExpression(Operators.Next, 
                        new Existence(Occurances - 1, LogEvent).GetExpression())));
        }
        
        public override string ToString() 
            => $"Existence({Occurances}, \"{LogEvent}\")";
    }
}