using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public struct NotChainSuccession: ITemplate
    {
        public string LogEventA;
        public string LogEventB;
        public const int NumberOfArguments = 2;
        
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
    }
}