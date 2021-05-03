using System.Reflection;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Models
{
    public class LtlExpression
    {
        public LtlExpression(Operators @operator, LtlExpression inner)
        {
            if ((int) @operator > 255)
                throw new TargetParameterCountException("Unary operators take only 1 expression as parameter");
            Operator = @operator;
            InnerLeft = inner;
        }
        
        public LtlExpression(Operators @operator, LtlExpression innerLeft, LtlExpression innerRight)
        {
            Operator = @operator;
            InnerLeft = innerLeft;
            InnerRight = innerRight;
        }
        
        public LtlExpression(string atom)
        {
            Operator = Operators.None;
            Atom = atom;
        }

        public Operators Operator { get; }

        public LtlExpression InnerLeft { get; }
        public LtlExpression InnerRight { get; }
        
        public string Atom { get; }
    }
}