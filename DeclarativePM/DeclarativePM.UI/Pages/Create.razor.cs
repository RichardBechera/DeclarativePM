using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeclarativePM.Lib.Declare_Templates.Factories;
using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.UI.Data;
using DeclarativePM.UI.Enums;
using DeclarativePM.UI.Utils;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Create : ComponentBase
    {
        
        private CreateMethod method = CreateMethod.Undefined;
        private bool chooseMethod = true;
        private bool chooseLog = false;
        private bool chooseModel = false;
        private EventLog _selectedLog;

        public ITemplate SelectedTemplateInstance;
        public CreateTemplateWrap CurrentlyEditedTemplate;

        private List<ParametrizedTemplate> templates;
        private List<ITemplate> currentTemplates = new();
        private ParametrizedTemplate current;
        public List<string> activities;
        public TreeNodeModel treeTemplates;
        private DeclareModel _declareModel;
        
        TemplateInstanceType[] value2Items = Enum.GetValues(typeof(TemplateInstanceType))
            .Cast<TemplateInstanceType>().Where(x => x != TemplateInstanceType.None).ToArray();

        
        public async Task ChooseMethod(CreateMethod val)
        {
            if (val == CreateMethod.Create)
            {
                templates = new();
                activities = new();
                Utilities.CreateTreeNode(out treeTemplates, templates);
            }

            method = val;
            chooseMethod = false;
            if (val == CreateMethod.CreateWithLog)
                chooseLog = true;
            else if (val == CreateMethod.Edit)
                chooseModel = true;
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task LogContinueCreate()
        {

            if (StateContainer.DeclareModels.Any(x => x.Log.Equals(_selectedLog)))
            {
                var result = await MatDialogService.AskAsync($"For {_selectedLog.Name} already existing" +
                                                             $" DECLARE model was found, would you like to import it?", new[] {"YES", "NO", "CANCEL"});
                if (result == "CANCEL")
                    return;
                
                if (result == "YES")
                {
                    chooseLog = false;
                    _declareModel = StateContainer.DeclareModels.Find(x => x.Log.Equals(_selectedLog));
                    await ModelContinueCreate();
                    await InvokeAsync(StateHasChanged);
                    return;
                }
            }

            chooseLog = false;
            activities = _selectedLog.Logs.Select(x => x.Activity).Distinct().ToList();
            templates = new();
            Utilities.CreateTreeNode(out treeTemplates, templates);
            currentTemplates = new();
            
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ModelContinueCreate()
        {
            chooseModel = false;
            templates = _declareModel.Constraints.ToList();
            activities = _declareModel.GetAllActivities();
            Utilities.CreateTreeNode(out treeTemplates, templates);
            
            await InvokeAsync(StateHasChanged);
        }

        public async Task AddActivity()
        {
            var result = await MatDialogService.PromptAsync("Name of activity: ");
            if (activities.Contains(result))
            {
                await MatDialogService.AlertAsync("Activity is already in the list");
                return;
            }
            activities.Add(result.Trim());
            await InvokeAsync(StateHasChanged);
        }

        public async Task RemoveActivity(string activity)
        {
            if (activities.Contains(activity))
                activities.Remove(activity);
            await InvokeAsync(StateHasChanged);
        }
        
        public string GetChipBackground(TemplateInstanceType tit)
        {
            if (current is not null && current.TemplateDescription.TemplateType == tit)
                return "background: #b6b6b6";
            return templates.Any(x => x.TemplateDescription.TemplateType == tit && x.TemplateInstances.Count > 0)
                ? "background: #ffd5ff" : "background: #f3f3f3";
        }

        public void SelectionChangedEvent(object row)
        {
            if (row is null)
            {
                SelectedTemplateInstance = null;
                CurrentlyEditedTemplate = new(current.TemplateDescription.TemplateType, current.TemplateDescription.TemplateParametersType);
            }
            else
            {
                SelectedTemplateInstance = (ITemplate) row;
                CurrentlyEditedTemplate = WrapSelection();
            }
            StateHasChanged();
        }

        public CreateTemplateWrap WrapSelection()
        {
            switch(current.TemplateDescription.TemplateParametersType)
            {
                case TemplateTypes.Existence:
                    ExistenceTemplate temp1 = (ExistenceTemplate) SelectedTemplateInstance;
                    return new(temp1.GetEvent(), temp1.GetCount(), 
                        current.TemplateDescription.TemplateType, current.TemplateDescription.TemplateParametersType);
                case TemplateTypes.BiTemplate:
                    BiTemplate temp2 = (BiTemplate) SelectedTemplateInstance;
                    return new(temp2.GetEventA(), temp2.GetEventB(),
                        current.TemplateDescription.TemplateType, current.TemplateDescription.TemplateParametersType);
                case TemplateTypes.UniTemplate:
                    UniTemplate temp3 = (UniTemplate) SelectedTemplateInstance;
                    return new(temp3.GetEventA(), current.TemplateDescription.TemplateType,
                        current.TemplateDescription.TemplateParametersType);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public async Task SelectedPTemplateChanged(MatChip selectedTemplate)
        {
            current = templates.Find(x => x.TemplateDescription.TemplateType == (TemplateInstanceType)selectedTemplate.Value);
            current ??= new ParametrizedTemplate((TemplateInstanceType) selectedTemplate.Value);
            currentTemplates = current.TemplateInstances;
            CurrentlyEditedTemplate = new(current.TemplateDescription.TemplateType, current.TemplateDescription.TemplateParametersType);
            await InvokeAsync(StateHasChanged);
        }

        public string GetValue1(ITemplate template)
        {
            if (current is null)
                return "";
            return current.TemplateDescription.TemplateParametersType switch
            {
                TemplateTypes.Existence =>
                    ((ExistenceTemplate) template).GetEvent(),
                TemplateTypes.BiTemplate =>
                    ((BiTemplate) template).GetEventA(),
                TemplateTypes.UniTemplate =>
                    ((UniTemplate) template).GetEventA(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public string GetValue2(ITemplate template)
        {
            if (current is null)
                return "";
            return current.TemplateDescription.TemplateParametersType switch
            {
                TemplateTypes.Existence => 
                    ((ExistenceTemplate) template).GetCount().ToString(),
                TemplateTypes.BiTemplate => 
                    ((BiTemplate) template).GetEventB(),
                TemplateTypes.UniTemplate => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetHeader(bool first)
        {
            if (current is null)
                return "";
            return current.TemplateDescription.TemplateParametersType switch
            {
                TemplateTypes.Existence => first ? "Event" : "Occurrences",
                TemplateTypes.BiTemplate => first ? "Event-A" : "Event-B",
                TemplateTypes.UniTemplate => first ? "Event" : "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void RemoveTemplate(ITemplate template)
        {
            foreach (var templateInstance in current.TemplateInstances)
            {
                if (templateInstance.GetExpression().ToString() == template.GetExpression().ToString())
                {
                    current.TemplateInstances.Remove(templateInstance);
                    break;
                }
            }
            Utilities.CreateTreeNode(out treeTemplates, templates);
            StateHasChanged();
        }

        public async Task SaveTemplate()
        {
            ITemplate template = CurrentlyEditedTemplate.TemplateTypes switch
            {
                TemplateTypes.UniTemplate 
                    => UniTemplateFactory.GetInstance(CurrentlyEditedTemplate.TemplateInstanceType,
                        CurrentlyEditedTemplate.EventA.Trim()),
                TemplateTypes.BiTemplate
                    => BiTemplateFactory.GetInstance(CurrentlyEditedTemplate.TemplateInstanceType,
                        CurrentlyEditedTemplate.EventA.Trim(), CurrentlyEditedTemplate.EventB.Trim()),
                TemplateTypes.Existence
                    => ExistenceFactory.GetInstance(CurrentlyEditedTemplate.TemplateInstanceType,
                        CurrentlyEditedTemplate.Occurrences, CurrentlyEditedTemplate.EventA),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (current.TemplateInstances.Exists(t => t.GetExpression().ToString() == template.GetExpression().ToString()))
            {
                await MatDialogService.AlertAsync("This template already exists in the list");
                return;
            }

            current.TemplateInstances.Add(template);
            if(!templates.Contains(current))
                templates.Add(current);
            Utilities.CreateTreeNode(out treeTemplates, templates);
            await InvokeAsync(StateHasChanged);
        }

        public bool DisableSave()
        {
            return CurrentlyEditedTemplate.TemplateTypes switch
            {
                TemplateTypes.Existence => !activities.Exists(x => CurrentlyEditedTemplate.EventA == x),
                TemplateTypes.BiTemplate => !(activities.Exists(x => CurrentlyEditedTemplate.EventA == x) 
                                            && activities.Exists(x => CurrentlyEditedTemplate.EventB == x)),
                TemplateTypes.UniTemplate => !activities.Exists(x => CurrentlyEditedTemplate.EventA == x),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public async Task SaveModel()
        {
            var result = await MatDialogService.PromptAsync("Name of the DECLARE model", _declareModel?.Name ?? "DEFAULT MODEL NAME");
            if (_declareModel is null)
            {
                DeclareModel md = new DeclareModel(result.Trim(), templates);
                _declareModel = md;
                StateContainer.DeclareModels.Add(md);
            }
            else
            {
                _declareModel.Constraints = templates;
            }
        }
    }
}