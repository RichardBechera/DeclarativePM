using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Chain Precedence template
    /// Each time B occurs, then A occurs immediately before
    /// subsequent(next(B) => A)
    /// </summary>
    public struct ChainPrecedence: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public ChainPrecedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(next(B) => A)
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB)),
                new LtlExpression(LogEventA)));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventB);
        
        public LtlExpression GetExpressionWithWitness()
        {
            //phi && eventual(B)
            return new LtlExpression(Operators.And, GetExpression(),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"ChainPrecedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}