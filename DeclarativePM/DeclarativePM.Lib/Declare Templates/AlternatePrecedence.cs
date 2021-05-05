using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct AlternatePrecedence: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public AlternatePrecedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};
        
        public LtlExpression GetExpression()
        {
            //precedence(A, B) && subsequent(B => next(precedence(A, B)))
            return new LtlExpression(Operators.And, new Precedence(LogEventA, LogEventB).GetExpression(),
                new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                    new LtlExpression(LogEventB), new LtlExpression(Operators.Next,
                        new Precedence(LogEventA, LogEventB).GetExpression()))));
        }
    }
}