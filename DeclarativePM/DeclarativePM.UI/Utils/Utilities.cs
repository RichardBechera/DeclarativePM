using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;

namespace DeclarativePM.UI.Utils
{
    public static class Utilities
    {
        public static void CreateTreeNode(out TreeNodeModel treeTemplates, List<ParametrisedTemplate> templates)
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

        private static TreeNodeModel GenerateInnerNodes(TemplateBookType tbt, string name, List<ParametrisedTemplate> templates)
        {
            return new()
            {
                Name = name,
                Nodes = templates.Where(x => x.Template.GetTemplateBookType() == tbt)
                    .Select(template =>
                    {
                        return new TreeNodeModel()
                        {
                            Name = template.Template.ToString(),
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
    }
}