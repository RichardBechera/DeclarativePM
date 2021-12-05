using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public abstract class UniTemplate : ITemplate
    {
        public readonly string LogEvent;

        protected UniTemplate(string logEvent)
        {
            LogEvent = logEvent;
        }

        public string GetEventA() 
            => LogEvent;
        public abstract LtlExpression GetExpression();
    }
}