using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Response: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public Response(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //subsequent(A => eventual(B))
            return new LtlExpression(Operators.Subsequent,
                new LtlExpression(Operators.Imply, new LtlExpression(LogEventA),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"Response(\"{LogEventA}\", \"{LogEventB}\")";
    }
}