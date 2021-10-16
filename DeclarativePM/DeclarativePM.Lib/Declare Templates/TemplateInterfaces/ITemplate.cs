using System;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates
{
    public interface ITemplate
    {
        public static int GetAmountOfArguments() => -1;

        public static Type[] GetConstructorOptions() => null;

        public LtlExpression GetExpression();
    }
}