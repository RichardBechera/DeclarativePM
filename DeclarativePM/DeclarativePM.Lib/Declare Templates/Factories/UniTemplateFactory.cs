using System;
using DeclarativePM.Lib.Declare_Templates.AbstractClasses;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Declare_Templates.Factories
{
    public static class UniTemplateFactory
    {
        public static UniTemplate GetInstance(TemplateInstanceType type, string evnt)
        {
            switch (type)
            {
                case TemplateInstanceType.Init:
                    return new Init(evnt);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}