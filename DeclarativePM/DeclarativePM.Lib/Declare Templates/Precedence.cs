using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Precedence: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public Precedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //(!B U A) || subsequent(!B)
            return new LtlExpression(Operators.Or, new LtlExpression(Operators.Least,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB)),
                    new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Subsequent,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB))));
        }
        
        public override string ToString() 
            => $"Precedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}