using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeclarativePM.Lib.Declare_Templates;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;
using DeclarativePM.UI.Enums;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Conformance : ComponentBase
    {
        public List<TraceDTO> Traces = new();
        public TraceDTO SelectedTrace;
        private DeclareModel _declareModel;
        private EventLog _selectedLog;
        public TreeNodeModel TreeNodeModel;
        private List<TraceDTO> _selectedTraces = new();
        public ConformancePageView View = ConformancePageView.Conformance;
        public bool seeActivities = false;
        public List<string> activities = new();
        public Event CurrentTraceEvent;
        private List<SimpleTemplateEvaluation> evaluations;
        private bool showResults = false;
        private SimpleTemplateEvaluation _evaluation;
        private ITemplate _constraint;
        private MatChip selectedChip;
        public async Task AddCases()
        {
            var result = await MatDialogService.AskAsync("Would you like to:", new string[] {"Create new trace", "Import traces from log", "Close"});
            if (result == "Close")
            {
                return;
            }

            if (result == "Create new trace")
            {
                SelectedTrace = new TraceDTO(new());
                SelectedTrace.Case = String.Empty;
                CurrentTraceEvent = new Event(activities?.FirstOrDefault(), SelectedTrace.Case);
                View = ConformancePageView.CreateTrace;
            }
            else if (result == "Import traces from log")
            {
                View = ConformancePageView.SelectLog;
            }

            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ShowCase()
        {
            if (View == ConformancePageView.SelectedTrace)
            {
                View = ConformancePageView.Conformance;
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (SelectedTrace is null)
            {
                await MatDialogService.AlertAsync("You have not selected any trace!");
                return;
            }

            View = ConformancePageView.SelectedTrace;
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ChangeModel()
        {
            View = ConformancePageView.SelectModel;
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ChooseTrace(TraceDTO trace)
        {
            SelectedTrace = trace;
            await InvokeAsync(StateHasChanged);
        }

        public string GetExpansionBackground(TraceDTO trace)
        {
            return trace is null || !trace.Equals(SelectedTrace) ? "background: #f3f3f3" : "background: #ffd5ff";
        }

        public async Task OnModelSelected()
        {
            View = ConformancePageView.Conformance;
            if (_declareModel is not null)
            {
                activities.AddRange(_declareModel.GetAllActivities());
                activities = activities.Distinct().ToList();
                Utils.Utilities.CreateTreeNode(out TreeNodeModel, _declareModel.Constraints);
            }

            await InvokeAsync(StateHasChanged);
        }
        
        public async Task OnLogSelected()
        {
            View = ConformancePageView.SelectTraces;
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task OnTracesSelected()
        {
            View = ConformancePageView.Conformance;
            foreach (var trace in _selectedTraces)
            {
                if(!Traces.Contains(trace))
                    Traces.Add(trace);
            }

            _selectedTraces = new();
            await InvokeAsync(StateHasChanged);
        }

        public List<TraceDTO> GetLogTraceDtos()
        {
            return _selectedLog?.GetAllTraces().Select(x => new TraceDTO(x)).ToList();
        }

        public async Task RemoveTrace(TraceDTO trace)
        {
            if (Traces.Contains(trace))
                Traces.Remove(trace);
            await InvokeAsync(StateHasChanged);
        }

        public void RemoveEventFromSelectedTrace(Event e)
        {
            if (e is not null && SelectedTrace.Events.Contains(e))
                SelectedTrace.Events.Remove(e);
            
            StateHasChanged();
        }

        public void SelectionChangedEvent(object e)
        {
            if (e is null)
            {
                CurrentTraceEvent = new(activities?.First() ?? "", SelectedTrace.Case);
            }
            else
            {
                CurrentTraceEvent = (Event)e;
            }
            StateHasChanged();
        }

        public async Task AddEvent()
        {
            CurrentTraceEvent.CaseId = SelectedTrace.Case;
            SelectedTrace.Events.Add(CurrentTraceEvent);

            CurrentTraceEvent = new(activities?.First() ?? "", SelectedTrace.Case);

            await InvokeAsync(StateHasChanged);
        }
        
        public async Task SaveTrace()
        {
            View = ConformancePageView.Conformance;
            if (!Traces.Contains(SelectedTrace))
                Traces.Add(SelectedTrace);

            await InvokeAsync(StateHasChanged);
        }

        public void SeeActivities()
        {
            seeActivities = !seeActivities;
            StateHasChanged();
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

        public async Task EvaluateWhole()
        {
            evaluations = MainMethods.EvaluateTrace(_declareModel, SelectedTrace.Events);
            showResults = true;
            _evaluation = evaluations.First();
            await InvokeAsync(StateHasChanged);
        }
        
        public string GetConformanceChipBackground(Event e)
        {
            EventActivationType type = _evaluation.evals[_constraint][e];

            return type switch
            {
                EventActivationType.None => "background: #d9d9d9",
                EventActivationType.Fulfilment => "background: #ff66ff",
                EventActivationType.Conflict => "background: #ffff00",
                EventActivationType.Violation => "background: #ff0000",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}