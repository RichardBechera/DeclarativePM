using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Alternate Precedence template
    /// Each time B occurs, it is preceded by A and no other B can recur in between
    /// precedence(A, B) && subsequent(B => next(precedence(A, B)))
    /// </summary>
    public struct AlternatePrecedence: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public AlternatePrecedence(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }
        
        public LtlExpression GetExpression()
        {
            //precedence(A, B) && subsequent(B => next(precedence(A, B)))
            return new LtlExpression(Operators.And, new Precedence(LogEventA, LogEventB).GetExpression(),
                new LtlExpression(Operators.Subsequent, new LtlExpression(Operators.Imply,
                    new LtlExpression(LogEventB), new LtlExpression(Operators.Next,
                        new Precedence(LogEventA, LogEventB).GetExpression()))));
        }

        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventB);

        public LtlExpression GetVacuityCondition()
        {
            //eventual(B)
            return new LtlExpression(Operators.Eventual, new LtlExpression(LogEventB));
        }

        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;

        public override string ToString() 
            => $"AlternatePrecedence(\"{LogEventA}\", \"{LogEventB}\")";
    }
}