using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct ChainResponse: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public ChainResponse(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //subsequent(A => next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
        }
    }
}