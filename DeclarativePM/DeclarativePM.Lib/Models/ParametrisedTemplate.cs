using System;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Utils;

namespace DeclarativePM.Lib.Models
{
    public readonly struct ParametrisedTemplate
    {
        public Type Template { get; }
        public ITemplate TemplateInstance { get; }
        public decimal Poe { get; }
        public decimal Poi { get; }

        public ParametrisedTemplate(Type template, decimal poe = 100, decimal poi = 100)
        {
            if (!(template.IsValueType && template.IsAssignableTo(typeof(ITemplate))))
                throw new ArgumentException("Type has to be ValueType implementing ITemplate interface");
            Template = template;
            TemplateInstance = null;
            UtilMethods.CutIntoRange(ref poe, 1, 100);
            Poe = poe;
            UtilMethods.CutIntoRange(ref poi, 1, 100);
            Poi = poi;
        }
        
        public ParametrisedTemplate(Type template, ITemplate templateInstance, decimal poe = 100, decimal poi = 100)
        {
            if (!(template.IsValueType && template.IsAssignableTo(typeof(ITemplate))))
                throw new ArgumentException("Type has to be ValueType implementing ITemplate interface");
            Template = template;
            TemplateInstance = templateInstance;
            UtilMethods.CutIntoRange(ref poe, 1, 100);
            Poe = poe;
            UtilMethods.CutIntoRange(ref poi, 1, 100);
            Poi = poi;
        }
    }
}