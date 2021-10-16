using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Succession: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public Succession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public static int GetAmountOfArguments() => 2;

        public static Type[] GetConstructorOptions() => new[] {typeof(string), typeof(string)};

        public LtlExpression GetExpression()
        {
            //response(A, B) && precedence(A, B)
            return new LtlExpression(Operators.And, new Response(LogEventA, LogEventB).GetExpression(),
                new Precedence(LogEventA, LogEventB).GetExpression());
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public override string ToString() 
            => $"Succession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}