using System;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Declare_Templates.Factories
{
    public static class UniTemplateFactory
    {
        public static IUniTemplate GetInstance(TemplateInstanceType type, string evnt)
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