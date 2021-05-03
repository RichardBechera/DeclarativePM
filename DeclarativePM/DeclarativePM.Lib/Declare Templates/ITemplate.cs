using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public interface ITemplate
    {
        public LtlExpression GetExpression();
    }
}