using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public interface IVacuityDetection
    {
        public LtlExpression GetVacuityCondition();

        public LtlExpression GetWitnessExpression();
    }
}