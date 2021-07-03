using System;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.Factories;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Utils
{
    public static class ExtensionMethods
    {
        public static TemplateInstanceType GetPossibleTemplateType(this Type type)
        {
            if (type.IsAssignableTo(typeof(Absence)))
                return TemplateInstanceType.Absence;
            if (type.IsAssignableTo(typeof(AlternatePrecedence)))
                return TemplateInstanceType.AlternatePrecedence;
            if (type.IsAssignableTo(typeof(AlternateResponse)))
                return TemplateInstanceType.AlternateResponse;
            if (type.IsAssignableTo(typeof(AlternateSuccession)))
                return TemplateInstanceType.AlternateSuccession;
            if (type.IsAssignableTo(typeof(ChainPrecedence)))
                return TemplateInstanceType.ChainPrecedence;
            if (type.IsAssignableTo(typeof(ChainResponse)))
                return TemplateInstanceType.ChainResponse;
            if (type.IsAssignableTo(typeof(ChainSuccession)))
                return TemplateInstanceType.ChainSuccession;
            if (type.IsAssignableTo(typeof(Coexistence)))
                return TemplateInstanceType.Coexistence;
            if (type.IsAssignableTo(typeof(Exactly)))
                return TemplateInstanceType.Exactly;
            if (type.IsAssignableTo(typeof(Existence)))
                return TemplateInstanceType.Existence;
            if (type.IsAssignableTo(typeof(Init)))
                return TemplateInstanceType.Init;
            if (type.IsAssignableTo(typeof(NotChainSuccession)))
                return TemplateInstanceType.NotChainSuccession;
            if (type.IsAssignableTo(typeof(NotCoexistence)))
                return TemplateInstanceType.NotCoexistence;
            if (type.IsAssignableTo(typeof(NotSuccession)))
                return TemplateInstanceType.NotSuccession;
            if (type.IsAssignableTo(typeof(Precedence)))
                return TemplateInstanceType.Precedence;
            if (type.IsAssignableTo(typeof(RespondedExistence)))
                return TemplateInstanceType.RespondedExistence;
            if (type.IsAssignableTo(typeof(Response)))
                return TemplateInstanceType.Response;
            if (type.IsAssignableTo(typeof(Succession)))
                return TemplateInstanceType.Succession;
            return TemplateInstanceType.None;
        }

        public static TemplateTypes GetTemplateType(this TemplateInstanceType template)
        {
            switch (template)
            {
                case TemplateInstanceType.Init:
                    return TemplateTypes.UniTemplate;
                case TemplateInstanceType.Exactly:
                case TemplateInstanceType.Existence:
                case TemplateInstanceType.Absence:
                    return TemplateTypes.Existence;
                case TemplateInstanceType.AlternatePrecedence:
                case TemplateInstanceType.AlternateResponse:
                case TemplateInstanceType.AlternateSuccession:
                case TemplateInstanceType.ChainPrecedence:
                case TemplateInstanceType.ChainResponse:
                case TemplateInstanceType.ChainSuccession:
                case TemplateInstanceType.Coexistence:
                case TemplateInstanceType.NotChainSuccession:
                case TemplateInstanceType.NotCoexistence:
                case TemplateInstanceType.NotSuccession:
                case TemplateInstanceType.Precedence:
                case TemplateInstanceType.RespondedExistence:
                case TemplateInstanceType.Response:
                case TemplateInstanceType.Succession:
                    return TemplateTypes.BiTemplate;
                default:
                    return TemplateTypes.None;
            }
        }
        
        public static int GetTemplateEventArgs(this TemplateTypes type)
        {
            switch (type)
            {
                case TemplateTypes.UniTemplate:
                case TemplateTypes.Existence:
                    return 1;
                case TemplateTypes.BiTemplate:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}