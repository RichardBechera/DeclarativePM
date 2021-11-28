using System;
using System.Collections.Generic;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;

namespace DeclarativePM.Lib.Declare_Templates
{
    public interface ITemplate
    {
        public static int GetAmountOfArguments() => -1;

        public static Type[] GetConstructorOptions() => null;

        public LtlExpression GetExpression();
    }
}