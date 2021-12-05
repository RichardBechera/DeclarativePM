using System;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeclarativePM.Lib.IO
{
    public class TemplateConverter: JsonConverter<ITemplate>
    {
        private readonly TemplateInstanceType _type = TemplateInstanceType.None;

        public TemplateConverter(TemplateInstanceType type)
        {
            _type = type;
        }

        public TemplateConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, ITemplate value, JsonSerializer serializer)
        {
            JObject.FromObject(value).WriteTo(writer);
        }

        public override ITemplate ReadJson(JsonReader reader, Type objectType, ITemplate existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return _type switch
            {
                TemplateInstanceType.Absence => jo.ToObject<Absence>(),
                TemplateInstanceType.AlternatePrecedence => jo.ToObject<AlternatePrecedence>(),
                TemplateInstanceType.AlternateResponse => jo.ToObject<AlternateResponse>(),
                TemplateInstanceType.AlternateSuccession => jo.ToObject<AlternateSuccession>(),
                TemplateInstanceType.ChainPrecedence => jo.ToObject<ChainPrecedence>(),
                TemplateInstanceType.ChainResponse => jo.ToObject<ChainResponse>(),
                TemplateInstanceType.ChainSuccession => jo.ToObject<ChainSuccession>(),
                TemplateInstanceType.Coexistence => jo.ToObject<Coexistence>(),
                TemplateInstanceType.Exactly => jo.ToObject<Exactly>(),
                TemplateInstanceType.Existence => jo.ToObject<Existence>(),
                TemplateInstanceType.Init => jo.ToObject<Init>(),
                TemplateInstanceType.NotChainSuccession => jo.ToObject<NotChainSuccession>(),
                TemplateInstanceType.NotCoexistence => jo.ToObject<NotCoexistence>(),
                TemplateInstanceType.NotSuccession => jo.ToObject<NotSuccession>(),
                TemplateInstanceType.Precedence => jo.ToObject<Precedence>(),
                TemplateInstanceType.RespondedExistence => jo.ToObject<RespondedExistence>(),
                TemplateInstanceType.Response => jo.ToObject<Response>(),
                TemplateInstanceType.Succession => jo.ToObject<Succession>(),
                _ => null
            };
        }
    }
}