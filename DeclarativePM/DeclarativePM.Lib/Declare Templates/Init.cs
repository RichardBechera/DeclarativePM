using DeclarativePM.Lib.Declare_Templates.AbstractClasses;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    /// <summary>
    /// LTL Init template
    /// A is the first to occur
    /// A
    /// </summary>
    public class Init: UniTemplate
    {


        public Init(string logEvent) : base(logEvent)
        {
        }

        public override LtlExpression GetExpression()
        {
            //A
            return new LtlExpression(LogEvent);
        }

        public override string ToString() 
            => $"Init(\"{LogEvent}\")";
    }
}