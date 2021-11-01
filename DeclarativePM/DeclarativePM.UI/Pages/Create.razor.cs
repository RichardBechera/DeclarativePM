using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Declare_Templates.Factories;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
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

        private List<ParametrisedTemplate> templates;
        private List<ITemplate> currentTemplates = new();
        private ParametrisedTemplate current;
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
            activities = new();
            templates = new();
            Utilities.CreateTreeNode(out treeTemplates, templates);
            currentTemplates = new();
            
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ModelContinueCreate()
        {
            chooseModel = false;
            templates = _declareModel.Constraints;
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
            activities.Add(result);
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
            if (current is not null && current.Template == tit)
                return "background: #b6b6b6";
            return templates.Any(x => x.Template == tit && x.TemplateInstances.Count > 0)
                ? "background: #ffd5ff" : "background: #f3f3f3";
        }

        public void SelectionChangedEvent(object row)
        {
            if (row is null)
            {
                SelectedTemplateInstance = null;
                CurrentlyEditedTemplate = new(current.Template, current.TemplateType);
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
            switch(current.TemplateType)
            {
                case TemplateTypes.Existence:
                    IExistenceTemplate temp1 = (IExistenceTemplate) SelectedTemplateInstance;
                    return new(temp1.GetEvent(), temp1.GetCount(), current.Template, current.TemplateType);
                case TemplateTypes.BiTemplate:
                    IBiTemplate temp2 = (IBiTemplate) SelectedTemplateInstance;
                    return new(temp2.GetEventA(), temp2.GetEventB(), current.Template, current.TemplateType);
                case TemplateTypes.UniTemplate:
                    IUniTemplate temp3 = (IUniTemplate) SelectedTemplateInstance;
                    return new(temp3.GetEventA(), current.Template, current.TemplateType);
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public async Task SelectedPTemplateChanged(MatChip selectedTemplate)
        {
            current = templates.Find(x => x.Template == (TemplateInstanceType)selectedTemplate.Value);
            current ??= new ParametrisedTemplate((TemplateInstanceType) selectedTemplate.Value);
            currentTemplates = current.TemplateInstances;
            CurrentlyEditedTemplate = new(current.Template, current.TemplateType);
            await InvokeAsync(StateHasChanged);
        }

        public string GetValue1(ITemplate template)
        {
            if (current is null)
                return "";
            return current.TemplateType switch
            {
                TemplateTypes.Existence =>
                    ((IExistenceTemplate) template).GetEvent(),
                TemplateTypes.BiTemplate =>
                    ((IBiTemplate) template).GetEventA(),
                TemplateTypes.UniTemplate =>
                    ((IUniTemplate) template).GetEventA(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public string GetValue2(ITemplate template)
        {
            if (current is null)
                return "";
            return current.TemplateType switch
            {
                TemplateTypes.Existence => 
                    ((IExistenceTemplate) template).GetCount().ToString(),
                TemplateTypes.BiTemplate => 
                    ((IBiTemplate) template).GetEventB(),
                TemplateTypes.UniTemplate => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetHeader(bool first)
        {
            if (current is null)
                return "";
            return current.TemplateType switch
            {
                TemplateTypes.Existence => first ? "Event" : "Occurances",
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
                        CurrentlyEditedTemplate.EventA),
                TemplateTypes.BiTemplate
                    => BiTemplateFactory.GetInstance(CurrentlyEditedTemplate.TemplateInstanceType,
                        CurrentlyEditedTemplate.EventA, CurrentlyEditedTemplate.EventB),
                TemplateTypes.Existence
                    => ExistenceFactory.GetInstance(CurrentlyEditedTemplate.TemplateInstanceType,
                        CurrentlyEditedTemplate.Occurances, CurrentlyEditedTemplate.EventA),
                _ => throw new ArgumentOutOfRangeException()
            };
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
                DeclareModel md = new DeclareModel(result, templates);
                _declareModel = md;
                StateContainer.DeclareModels.Add(md);
            }
        }
    }
}