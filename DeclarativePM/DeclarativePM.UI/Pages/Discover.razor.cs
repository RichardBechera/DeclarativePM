using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Discover : ComponentBase
    {
        public EventLog SelectedLog;
            bool selectLog = true;
            bool selectParameters = false;
            bool configureTemplates = false;
            bool showDiscovered = false;
        
            TemplateInstanceType[] value2Items = Enum.GetValues(typeof(TemplateInstanceType))
                .Cast<TemplateInstanceType>().Where(x => x != TemplateInstanceType.None).ToArray();
            
            MatChip[] selectedTemplates;
            private List<ParametrisedTemplate> templates;
            public TreeNodeModel treeTemplates;

            public void Selection(EventLog log)
            {
                SelectedLog = log;
                StateHasChanged();
            }
        
            public async Task ContinueSelection()
            {
                selectLog = false;
                selectParameters = true;
                await InvokeAsync(StateHasChanged);
            }
            
            public async Task BackSelectionT()
            {
                selectLog = true;
                selectParameters = false;
                await InvokeAsync(StateHasChanged);
            }
            
            public async Task ContinueSelectionT()
            {
                selectParameters = false;
                configureTemplates = true;
                CreateTemplates();
                await InvokeAsync(StateHasChanged);
            }
            
            public async Task BackConfigure()
            {
                configureTemplates = false;
                selectParameters = true;
                await InvokeAsync(StateHasChanged);
            }
            
            public async Task ContinueConfigure()
            {
                configureTemplates = false;
                showDiscovered = true;
                ModelDiscovery();
                CreateTreeNode();
                await InvokeAsync(StateHasChanged);
            }
            
            public async Task BackDiscover()
            {
                configureTemplates = true;
                showDiscovered = false;
                await InvokeAsync(StateHasChanged);
            }
        
            public string GetExpansionBackground(EventLog el)
            {
                return el.Equals(SelectedLog) ? "background: #ffd5ff" : "background: #f3f3f3";
            }

            private void CreateTemplates()
            {
                bool isNew = templates is null;
                templates ??= new List<ParametrisedTemplate>();
                if (!isNew)
                {
                    templates = templates
                        .Where(x => selectedTemplates
                            .Any(y => x.Template == (TemplateInstanceType) y.Value))
                        .ToList();
                }

                foreach (var tit in selectedTemplates.Select(x => x.Value).Cast<TemplateInstanceType>())
                {
                    if (!isNew && templates.Any(x => x.Template == tit))
                        continue;
                    templates.Add(new(tit));
                }
            }

            public void CreateTreeNode()
            {
                treeTemplates = new()
                {
                    Name = "Discovered Constraints",
                    Nodes = new []
                    {
                        GenerateInnerNodes(TemplateBookType.Existential, "Existential"),
                        GenerateInnerNodes(TemplateBookType.Relational, "Relational"),
                        GenerateInnerNodes(TemplateBookType.NotRelational, "Not Relational")
                        
                    }
                };
            }

            private TreeNodeModel GenerateInnerNodes(TemplateBookType tbt, string name)
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

            public void ModelDiscovery()
            {
                var disco = new Discovery();
                disco.DiscoverModel(SelectedLog, templates);
            }

            public bool IsSelectedTit(TemplateInstanceType tit)
            {
                return selectedTemplates?.Any(x => ((TemplateInstanceType) x.Value) == tit) ?? false;
            }

            public bool GetDefaultCheckValue(TemplateInstanceType tit)
            {
                switch(tit)
                {
                    case TemplateInstanceType.Response:
                    case TemplateInstanceType.Succession:
                    case TemplateInstanceType.NotCoexistence:
                    case TemplateInstanceType.Precedence:
                    case TemplateInstanceType.Coexistence:
                        return true;
                    default:
                        return false;
        
                };
            }
        
        
            public void Dispose()
            {
            }
    }
}