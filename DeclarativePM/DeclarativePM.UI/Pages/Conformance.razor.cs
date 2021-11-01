using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeclarativePM.Lib.Models;
using DeclarativePM.UI.Data;
using DeclarativePM.UI.Enums;
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
        public ConformancePageView View = ConformancePageView.CreateTrace;
        public async Task AddCases()
        {
            var result = await MatDialogService.AskAsync("Would you like to:", new string[] {"Create new trace", "Import traces from log", "Close"});
            if (result == "Close")
                return;
            if (result == "Create new trace")
                View = ConformancePageView.CreateTrace;
            else if (result == "Import traces from log")
                View = ConformancePageView.SelectLog;
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task ShowCase()
        {
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
                Utils.Utilities.CreateTreeNode(out TreeNodeModel, _declareModel.Constraints);
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
    }
}