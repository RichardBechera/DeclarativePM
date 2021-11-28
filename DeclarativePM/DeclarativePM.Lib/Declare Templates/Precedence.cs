using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Precedence template
    /// B occurs only if preceded by A
    /// (!B U A) || subsequent(!B)
    /// </summary>
    public struct Precedence: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public Precedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //(!B U A) || subsequent(!B)
            return new LtlExpression(Operators.Or, new LtlExpression(Operators.Least,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB)),
                    new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Subsequent,
                    new LtlExpression(Operators.Not, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventB);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"Precedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}