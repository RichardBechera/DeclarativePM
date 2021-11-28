using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
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
        public int Occurances;
        public string LogEvent;
        
        public Exactly(int occurances, string logEvent)
        {
            Occurances = occurances;
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            //existence(n, A) && absence(n + 1, A)
            return new LtlExpression(Operators.And, new Existence(Occurances, LogEvent).GetExpression(),
                new Absence(Occurances + 1, LogEvent).GetExpression());
        }
        
        public override string ToString() 
            => $"Exactly({Occurances}, \"{LogEvent}\")";
        
        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurances;
    }
}