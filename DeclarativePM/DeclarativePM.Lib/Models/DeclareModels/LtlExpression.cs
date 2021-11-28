using System;
using System.Reflection;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Models.DeclareModels
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

        public override string ToString()
        {
            return Operator switch
            {
                Operators.None => Atom,
                Operators.Not => $"!({InnerLeft})",
                Operators.Next => $"next({InnerLeft})",
                Operators.Subsequent => $"subsequent({InnerLeft})",
                Operators.Eventual => $"Eventual({InnerLeft})",
                Operators.And => $"({InnerLeft} && {InnerRight})",
                Operators.Or => $"({InnerLeft} || {InnerRight})",
                Operators.Imply => $"({InnerLeft} => {InnerRight})",
                Operators.Equivalence => $"({InnerLeft} <=> {InnerRight})",
                Operators.Least => $"({InnerLeft} U {InnerRight})",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}