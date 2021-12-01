using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Init template
    /// A is the first to occur
    /// A
    /// </summary>
    public struct Init: IUniTemplate
    {
        public readonly string LogEvent;
        
        public Init(string logEvent)
        {
            LogEvent = logEvent;
        }

        public LtlExpression GetExpression()
        {
            //A
            return new LtlExpression(LogEvent);
        }

        public string GetEventA()
            => LogEvent;

        public override string ToString() 
            => $"Init(\"{LogEvent}\")";
    }
}