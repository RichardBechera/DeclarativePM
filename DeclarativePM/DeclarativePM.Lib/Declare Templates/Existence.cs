using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Existence template
    /// A occurs at least n times 
    /// Eventual(A && Next(Existence(n-1, A)))
    /// Eventual(A)
    /// </summary>
    public struct Existence: IExistenceTemplate
    {
        public readonly int Occurrences;
        public readonly string LogEvent;
        
        public Existence(int occurrences, string logEvent)
        {
            //what if 0 passed?
            Occurrences = occurrences;
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            if (Occurrences == 0)
            {
                //tautology a || !a
                return new LtlExpression(Operators.Or, 
                    new LtlExpression(LogEvent), 
                    new LtlExpression(Operators.Not, 
                        new LtlExpression(LogEvent)));
            }

            if (Occurrences == 1)
            {
                return new LtlExpression(Operators.Eventual, new LtlExpression(LogEvent));
            }
            //Eventual(A && Next(Existence(n-1, A)))
            return new LtlExpression(Operators.Eventual,
                new LtlExpression(Operators.And, 
                    new LtlExpression(LogEvent),
                    new LtlExpression(Operators.Next, 
                        new Existence(Occurrences - 1, LogEvent).GetExpression())));
        }
        
        public override string ToString() 
            => $"Existence({Occurrences}, \"{LogEvent}\")";
        
        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurrences;
    }
}