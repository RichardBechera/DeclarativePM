using System;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
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
        public int Occurances;
        public string LogEvent;
        
        public Absence(int occurances, string logEvent)
        {
            LogEvent = logEvent;
            Occurances = occurances;
        }

        public LtlExpression GetExpression()
        {
            //!existence(n, a)
            return new LtlExpression(Operators.Not, new Existence(Occurances, LogEvent).GetExpression());
        }

        public override string ToString() 
            => $"Absence({Occurances}, \"{LogEvent}\")";

        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurances;
    }
}