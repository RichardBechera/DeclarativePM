using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public abstract class ExistenceTemplate : ITemplate
    {
        public readonly int Occurrences;
        public readonly string LogEvent;

        protected ExistenceTemplate(int occurrences, string logEvent)
        {
            Occurrences = occurrences;
            LogEvent = logEvent;
        }
        
        public string GetEvent()
            => LogEvent;

        public int GetCount()
            => Occurrences;
        public abstract LtlExpression GetExpression();
    }
}