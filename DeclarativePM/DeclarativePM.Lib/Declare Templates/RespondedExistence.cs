using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Responded Existence template
    /// If A occurs, then B occurs 
    /// eventual(A) => eventual(B)
    /// </summary>
    public struct RespondedExistence: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public RespondedExistence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //eventual(A) => eventual(B)
            return new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"RespondedExistence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}