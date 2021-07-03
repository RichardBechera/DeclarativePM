using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct ChainPrecedence: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public ChainPrecedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //subsequent(next(B) => A)
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB)),
                new LtlExpression(LogEventA)));
        }
        
        public override string ToString() 
            => $"ChainPrecedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}