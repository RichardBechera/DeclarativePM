using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Exactly template
    /// A occurs exactly n times
    /// existence(n, A) && absence(n + 1, A)
    /// </summary>
    public struct Exactly: IExistenceTemplate
    {
        public readonly int Occurrences;
        public readonly string LogEvent;
        
        public Exactly(int occurrences, string logEvent)
        {
            Occurrences = occurrences;
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            //existence(n, A) && absence(n + 1, A)
            return new LtlExpression(Operators.And, new Existence(Occurrences, LogEvent).GetExpression(),
                new Absence(Occurrences + 1, LogEvent).GetExpression());
        }
        
        public override string ToString() 
            => $"Exactly({Occurrences}, \"{LogEvent}\")";
        
        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurrences;
    }
}