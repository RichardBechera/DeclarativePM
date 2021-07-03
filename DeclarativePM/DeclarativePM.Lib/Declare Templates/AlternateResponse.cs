using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct AlternateResponse: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public AlternateResponse(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //subsequent(A => next(!A U B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(LogEventA), new LtlExpression(Operators.Next,
                    new LtlExpression(Operators.Least, new LtlExpression(Operators.Not, 
                            new LtlExpression(LogEventA)),
                        new LtlExpression(LogEventB)))));
        }
        
        public override string ToString() 
            => $"AlternateResponse(\"{LogEventA}\", \"{LogEventB}\")";
    }
}