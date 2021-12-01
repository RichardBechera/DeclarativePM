using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Alternate Succession template
    /// A and B occur if and only if the latter follows the former, and they alternate each other
    /// alternateResponse(A, B) && AlternatePrecedence(A, B)
    /// </summary>
    public struct AlternateSuccession: IBiTemplate
    {
        public readonly string LogEventA;
        public readonly string LogEventB;
        
        public AlternateSuccession(string logEventA, string logEventB)
        {
            LogEventA = logEventA;
            LogEventB = logEventB;
        }

        public LtlExpression GetExpression()
        {
            //alternateResponse(A, B) && AlternatePrecedence(A, B)
            return new LtlExpression(Operators.And, new AlternateResponse(LogEventA, LogEventB).GetExpression(),
                new AlternatePrecedence(LogEventA, LogEventB).GetExpression());
        }
        
        public bool IsActivation(Event e)
            => e.Activity.Equals(LogEventA) || e.Activity.Equals(LogEventB);
        
        public string GetEventA()
            => LogEventA;

        public string GetEventB()
            => LogEventB;
        
        public override string ToString() 
            => $"AlternateSuccession(\"{LogEventA}\", \"{LogEventB}\")";
    }
}