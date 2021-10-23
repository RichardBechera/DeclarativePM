using System;
using System.Collections.Generic;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Models
{
    public class ParametrisedTemplate
    {
        public TemplateInstanceType Template { get; }
        public List<ITemplate> TemplateInstances { get; set; }
        public decimal Poe { get; set; }
        public decimal Poi { get; set; }

        public TemplateTypes TemplateType { get; }

        public ParametrisedTemplate(Type template, decimal poe = 100, decimal poi = 100) : this(template, new(), poe, poi)
        {
        }
        
        public ParametrisedTemplate(Type template, List<ITemplate> templateInstances, decimal poe = 100, decimal poi = 100)
        {
            if (!(template.IsValueType && template.IsAssignableTo(typeof(ITemplate))))
                throw new ArgumentException("Type has to be ValueType implementing ITemplate interface");
            Template = template.GetPossibleTemplateType();
            TemplateInstances = templateInstances;
            UtilMethods.CutIntoRange(ref poe, 1, 100);
            Poe = poe;
            UtilMethods.CutIntoRange(ref poi, 1, 100);
            Poi = poi;
            TemplateType = Template.GetTemplateType();
        }

        public ParametrisedTemplate(TemplateInstanceType template, decimal poe = 100, decimal poi = 100) : this(
            template, new(), poe, poi)
        {
        }


        public ParametrisedTemplate(TemplateInstanceType template, List<ITemplate> templateInstances, decimal poe = 100, decimal poi = 100)
        {
            Template = template;
            TemplateInstances = templateInstances;
            UtilMethods.CutIntoRange(ref poe, 1, 100);
            Poe = poe;
            UtilMethods.CutIntoRange(ref poi, 1, 100);
            Poi = poi;
            TemplateType = Template.GetTemplateType();
        }

        public ParametrisedTemplate(ParametrisedTemplate template, List<ITemplate> templateInstances)
        {
            Template = template.Template;
            TemplateInstances = templateInstances;
            Poe = template.Poe;
            Poi = template.Poi;
            TemplateType = template.TemplateType;
        }
    }
}