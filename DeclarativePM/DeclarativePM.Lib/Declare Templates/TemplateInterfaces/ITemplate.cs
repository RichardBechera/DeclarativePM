using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public interface ITemplate
    {
        public LtlExpression GetExpression();
    }
}