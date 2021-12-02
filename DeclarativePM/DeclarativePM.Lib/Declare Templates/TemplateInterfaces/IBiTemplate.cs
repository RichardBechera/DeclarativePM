using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public interface IBiTemplate : ITemplate
    {
        public bool IsActivation(Event e);

        public LtlExpression GetExpressionWithWitness();

        public string GetEventA();

        public string GetEventB();
    }
}