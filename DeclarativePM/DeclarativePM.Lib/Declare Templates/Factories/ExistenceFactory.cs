using System;
using DeclarativePM.Lib.Declare_Templates.AbstractClasses;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Declare_Templates.Factories
{
    public static class ExistenceFactory
    {
        public static ExistenceTemplate GetInstance(TemplateInstanceType type, int amount, string evnt)
        {
            switch (type)
            {
                case TemplateInstanceType.Absence:
                    return new Absence(amount, evnt);
                case TemplateInstanceType.Exactly:
                    return new Exactly(amount, evnt);
                case TemplateInstanceType.Existence:
                    return new Existence(amount, evnt);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}