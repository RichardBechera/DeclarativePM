using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Models.DeclareModels
{
    public readonly struct TemplateDescription
    {
        public string ReadableName { get; }
        public string Type { get; }
        public string Description { get; }
        public string LtlExpression { get; }
        public string Activations { get; }

        public TemplateTypes TemplateParametersType { get; }

        public TemplateBookType TemplateCategory { get; }

        public TemplateInstanceType TemplateType { get; }

        public TemplateDescription(string readableName, string type, string description, string ltlExpression, string activations, TemplateInstanceType templateType)
        {
            ReadableName = readableName;
            Type = type;
            Description = description;
            LtlExpression = ltlExpression;
            Activations = activations;
            TemplateType = templateType;
            TemplateCategory = templateType.GetTemplateBookType();
            TemplateParametersType = templateType.GetTemplateType();
        }
    }
}