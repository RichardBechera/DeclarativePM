using System;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Declare_Templates.Factories
{
    public static class BiTemplateFactory
    {
        public static IBiTemplate GetInstance(TemplateInstanceType type, string evnt1, string evnt2)
        {
            switch (type)
            {
                case TemplateInstanceType.AlternatePrecedence:
                    return new AlternatePrecedence(evnt1, evnt2);
                case TemplateInstanceType.AlternateResponse:
                    return new AlternateResponse(evnt1, evnt2);
                case TemplateInstanceType.AlternateSuccession:
                    return new AlternateSuccession(evnt1, evnt2);
                case TemplateInstanceType.ChainPrecedence:
                    return new ChainPrecedence(evnt1, evnt2);
                case TemplateInstanceType.ChainResponse:
                    return new ChainResponse(evnt1, evnt2);
                case TemplateInstanceType.ChainSuccession:
                    return new ChainSuccession(evnt1, evnt2);
                case TemplateInstanceType.Coexistence:
                    return new Coexistence(evnt1, evnt2);
                case TemplateInstanceType.NotChainSuccession:
                    return new NotChainSuccession(evnt1, evnt2);
                case TemplateInstanceType.NotCoexistence:
                    return new NotCoexistence(evnt1, evnt2);
                case TemplateInstanceType.NotSuccession:
                    return new NotSuccession(evnt1, evnt2);
                case TemplateInstanceType.Precedence:
                    return new Precedence(evnt1, evnt2);
                case TemplateInstanceType.RespondedExistence:
                    return new RespondedExistence(evnt1, evnt2);
                case TemplateInstanceType.Response:
                    return new Response(evnt1, evnt2);
                case TemplateInstanceType.Succession:
                    return new Succession(evnt1, evnt2);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}