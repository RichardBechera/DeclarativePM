using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Coexistence template
    /// If B occurs, then A occurs, and vice versa
    /// eventual(A) <=> eventual(B)
    /// </summary>
    public struct Coexistence: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public Coexistence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //eventual(A) <=> eventual(B)
            return new LtlExpression(Operators.Equivalence,
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB)));
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public LtlExpression GetExpressionWithWitness()
        {
            //phi && (eventual(A) || eventual(B))
            return new LtlExpression(Operators.And, GetExpression(),new LtlExpression(Operators.Or, 
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventA)),
                new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB))));
        }
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"Coexistence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}