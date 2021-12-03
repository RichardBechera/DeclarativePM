using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public interface IBiTemplate : ITemplate
    {
        public bool IsActivation(Event e);

        public LtlExpression GetVacuityCondition();
        
        public LtlExpression GetWitnessExpression() =>
            new LtlExpression(Operators.And, GetExpression(), GetVacuityCondition());

        public string GetEventA();

        public string GetEventB();
    }
}