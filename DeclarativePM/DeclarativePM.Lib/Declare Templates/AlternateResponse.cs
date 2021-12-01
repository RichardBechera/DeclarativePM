using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Alternate Response template
    /// Each time A occurs, then B occurs afterwards, before A recurs
    /// subsequent(A => next(!A U B))
    /// </summary>
    public struct AlternateResponse: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public AlternateResponse(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //subsequent(A => next(!A U B))
            return new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                new LtlExpression(LogEventA), new LtlExpression(Operators.Next,
                    new LtlExpression(Operators.Least, new LtlExpression(Operators.Not, 
                            new LtlExpression(LogEventA)),
                        new LtlExpression(LogEventB)))));
        }

        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;

        public override string ToString() 
            => $"AlternateResponse(\"{LogEventA}\", \"{LogEventB}\")";
    }
}