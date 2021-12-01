using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Chain Succession template
    /// A and B occur if and only if the latter immediately follows the former
    /// subsequent(A <=> next(B))
    /// </summary>
    public struct ChainSuccession: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public ChainSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A <=> next(B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Equivalence,
                new LtlExpression(LogEventA),
                new LtlExpression(Operators.Next, new LtlExpression(LogEventB))));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"ChainSuccession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}