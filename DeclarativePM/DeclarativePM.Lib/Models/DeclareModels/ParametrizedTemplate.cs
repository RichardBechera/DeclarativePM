using System;
using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Utils;
using Newtonsoft.Json;

namespace DeclarativePM.Lib.Models.DeclareModels
{
    public class ParametrizedTemplate
    {
        [JsonIgnore]
        public List<ITemplate> TemplateInstances { get; set; }

        [JsonIgnore] public List<ITemplate> OptionalConstraints { get; set; } = new();
        public decimal Poe { get; set; }
        public decimal Poi { get; set; }

        public bool CheckVacuously { get; set; } = true;
        [JsonIgnore]
        public TemplateDescription TemplateDescription { get; set; }

        [JsonConstructor]
        public ParametrizedTemplate(List<ITemplate> templateInstances, TemplateInstanceType templateType,
            decimal poe = 100, decimal poi = 100, bool checkVacuously = false)
        {
            TemplateInstances = templateInstances;
            Poe = UtilMethods.CutIntoRange(poe, 1, 100);
            Poi = UtilMethods.CutIntoRange(poi, 1, 100);
            CheckVacuously = checkVacuously;
            TemplateDescription = templateType.GetTemplateDescription();
        }

        public ParametrizedTemplate(Type template, decimal poe = 100, decimal poi = 100) : this(template, new(), poe, poi)
        {
        }
        
        public ParametrizedTemplate(Type template, List<ITemplate> templateInstances, decimal poe = 100, decimal poi = 100)
        {
            DescriptionFromType(template);
            TemplateInstances = templateInstances;
            Poe = UtilMethods.CutIntoRange(poe, 1, 100);
            Poi = UtilMethods.CutIntoRange(poi, 1, 100);
        }

        public ParametrizedTemplate(TemplateInstanceType template, decimal poe = 100, decimal poi = 100) : this(
            template, new(), poe, poi)
        {
        }


        public ParametrizedTemplate(TemplateInstanceType template, List<ITemplate> templateInstances, decimal poe = 100, decimal poi = 100)
        {
            TemplateDescription = template.GetTemplateDescription();
            TemplateInstances = templateInstances;
            Poe = UtilMethods.CutIntoRange(poe, 1, 100);
            Poi = UtilMethods.CutIntoRange(poi, 1, 100);
        }

        public ParametrizedTemplate(ParametrizedTemplate template, List<ITemplate> templateInstances)
        {
            TemplateDescription = template.TemplateDescription;
            TemplateInstances = templateInstances;
            Poe = template.Poe;
            Poi = template.Poi;
        }
        

        private void DescriptionFromType(Type template)
        {
            if (!template.IsAssignableTo(typeof(ITemplate)))
                throw new ArgumentException("Type has to be ValueType implementing ITemplate interface");
            var temp = template.GetPossibleTemplateType();
            TemplateDescription = temp.GetTemplateDescription();
        }
        

        public bool OrderMatters()
        {
            return TemplateDescription.TemplateType != TemplateInstanceType.Coexistence 
                   && TemplateDescription.TemplateType != TemplateInstanceType.NotCoexistence;
        }

        public void AddOptional(ITemplate template)
        {
            if(!OptionalConstraints.Contains(template))
                OptionalConstraints.Add(template);
            if(!TemplateInstances.Contains(template))
                TemplateInstances.Add(template);
        }
    }
}