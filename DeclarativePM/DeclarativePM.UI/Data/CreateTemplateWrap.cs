using DeclarativePM.Lib.Enums;

namespace DeclarativePM.UI.Data
{
    public class CreateTemplateWrap
    {
        public string EventA { get; set; } = "";
        public string EventB { get; set; } = "";
        public int Occurances { get; set; } = 0;
        public TemplateInstanceType TemplateInstanceType { get; set; }
        public TemplateTypes TemplateTypes { get; set; }

        public CreateTemplateWrap(TemplateInstanceType templateInstanceType, TemplateTypes templateTypes)
        {
            TemplateInstanceType = templateInstanceType;
            TemplateTypes = templateTypes;
        }

        public CreateTemplateWrap(string eventA, int occurances, TemplateInstanceType templateInstanceType, TemplateTypes templateTypes)
        {
            EventA = eventA;
            Occurances = occurances;
            TemplateInstanceType = templateInstanceType;
            TemplateTypes = templateTypes;
        }

        public CreateTemplateWrap(string eventA, string eventB, TemplateInstanceType templateInstanceType, TemplateTypes templateTypes)
        {
            EventA = eventA;
            EventB = eventB;
            TemplateInstanceType = templateInstanceType;
            TemplateTypes = templateTypes;
        }

        public CreateTemplateWrap(string eventA, TemplateInstanceType templateInstanceType, TemplateTypes templateTypes)
        {
            EventA = eventA;
            TemplateInstanceType = templateInstanceType;
            TemplateTypes = templateTypes;
        }
    }
        

    
}