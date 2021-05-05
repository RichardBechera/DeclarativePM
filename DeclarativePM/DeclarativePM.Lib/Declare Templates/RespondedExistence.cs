using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct RespondedExistence: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public RespondedExistence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //eventual(A) => eventual(B)
            return new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public override string ToString() 
            => $"RespondedExistence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}