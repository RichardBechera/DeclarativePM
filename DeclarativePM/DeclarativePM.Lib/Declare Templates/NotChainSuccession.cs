using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Not Chain Succession template
    /// A and B occur if and only if the latter does not immediately follow the former 
    /// subsequent(A => next(!B))
    /// </summary>
    public struct NotChainSuccession: IBiTemplate
    {
        public string LogEventA;
        public string LogEventB;
        
        public NotChainSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A => next(!B))
            return new LtlExpression(Operators.Subsequent, 
                new LtlExpression(Operators.Imply,
                    new LtlExpression(LogEventA), 
                    new LtlExpression(Operators.Eventual, 
                        new LtlExpression(Operators.Not,
                            new LtlExpression(LogEventB)))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"NotChainSuccession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}