using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;

namespace DeclarativePM.UI.Utils
{
    public static class Utilities
    {
        public static void CreateTreeNode(out TreeNodeModel treeTemplates, List<ParametrizedTemplate> templates)
        {
            treeTemplates = new()
            {
                Name = "Discovered Constraints",
                Nodes = new []
                {
                    GenerateInnerNodes(TemplateBookType.Existential, "Existential", templates),
                    GenerateInnerNodes(TemplateBookType.Relational, "Relational", templates),
                    GenerateInnerNodes(TemplateBookType.NotRelational, "Not Relational", templates)
                        
                }
            };
        }

        private static TreeNodeModel GenerateInnerNodes(TemplateBookType tbt, string name, List<ParametrizedTemplate> templates)
        {
            return new()
            {
                Name = name,
                Nodes = templates.Where(x => x.TemplateDescription.TemplateType.GetTemplateBookType() == tbt)
                    .Select(template =>
                    {
                        return new TreeNodeModel()
                        {
                            Name = template.TemplateDescription.TemplateType.ToString(),
                            Nodes = template.TemplateInstances.Select(instance => new TreeNodeModel()
                            {
                                Name = instance.ToString()
                            }).ToArray()
                        };
                    }).ToArray()

            };
        }
        
        public static string GetExpansionBackground<T>(T current, T selected)
        {
            //TODO colours into constants ?
            return current.Equals(selected) ? "background: #ffd5ff" : "background: #f3f3f3";
        }
        
        public static string GetExpansionBackground<T>(T current, List<T> from)
        {
            //TODO colours into constants ?
            return from.Contains(current) ? "background: #ffd5ff" : "background: #f3f3f3";
        }

        public static string GetTreeBackground(string current, List<SimpleTemplateEvaluation> evaluations)
        {
            if (evaluations is null)
                return "background: #ffffff";
            if (evaluations.Any(x => x.constraints.Any(y => y.ToString().Equals(current))))
            {
                return "background: #ffd5ff";
            }
            return "background: #ffffff";
        }
    }
}