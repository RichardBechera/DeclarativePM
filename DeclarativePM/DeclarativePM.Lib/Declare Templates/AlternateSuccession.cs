using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct AlternateSuccession: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public AlternateSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //alternateResponse(A, B) && AlternatePrecedence(A, B)
            return new LtlExpression(Operators.And, new AlternateResponse(LogEventA, LogEventB).GetExpression(),
                new AlternatePrecedence(LogEventA, LogEventB).GetExpression());
        }
        
        public override string ToString() 
            => $"AlternateSuccession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}