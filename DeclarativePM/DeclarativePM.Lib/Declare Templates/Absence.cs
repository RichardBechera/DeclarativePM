using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Absence template
    /// A occurs at most n - 1 times
    /// !existence(n, a)
    /// </summary>
    public struct Absence : IExistenceTemplate
    {
        public int Occurrences;
        public string LogEvent;
        
        public Absence(int occurrences, string logEvent)
        {
            if (occurrences < 1)
                throw new ArgumentException("Absence template parameter occurrences has to be higher or equal 1");
            LogEvent = logEvent;
            Occurrences = occurrences;
        }

        public LtlExpression GetExpression()
        {
            //!existence(n, a)
            return new LtlExpression(Operators.Not, new Existence(Occurrences, LogEvent).GetExpression());
        }

        public override string ToString() 
            => $"Absence({Occurrences}, \"{LogEvent}\")";

        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurrences;
    }
}