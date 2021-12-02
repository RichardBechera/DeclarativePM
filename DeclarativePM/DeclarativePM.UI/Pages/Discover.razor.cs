using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeclarativePM.Lib.Discovery;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;
using DeclarativePM.UI.Utils;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Discover : ComponentBase
    {
        private EventLog _selectedLog;
        bool selectLog = true;
        bool selectParameters = false;
        bool configureTemplates = false;
        bool showDiscovered = false;
        bool wait = false;
        bool abort = false;

        TemplateInstanceType[] value2Items = Enum.GetValues(typeof(TemplateInstanceType))
            .Cast<TemplateInstanceType>().Where(x => x != TemplateInstanceType.None).ToArray();

        private List<TemplateDescription> _templateDescriptions;

        MatChip[] selectedTemplates;
        private List<ParametrizedTemplate> templates;
        public TreeNodeModel treeTemplates;
        private DeclareModel _declareModel;
        CancellationTokenSource tokenSource = new();

        protected override void OnInitialized()
        {
            _templateDescriptions = value2Items.Select(e => e.GetTemplateDescription()).ToList();
            base.OnInitialized();
        }

        public void Selection(EventLog log)
        {
            _selectedLog = log;
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
            wait = true;
            await InvokeAsync(StateHasChanged);
            await ModelDiscoveryAsync();
            wait = false;
            Utilities.CreateTreeNode(out treeTemplates, templates);

            configureTemplates = false;
            showDiscovered = true;
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
            return el.Equals(_selectedLog) ? "background: #ffd5ff" : "background: #f3f3f3";
        }

        private void CreateTemplates()
        {
            bool isNew = templates is null;
            templates ??= new List<ParametrizedTemplate>();
            if (!isNew)
            {
                templates = templates
                    .Where(x => selectedTemplates
                        .Any(y => x.TemplateDescription.TemplateType == (TemplateInstanceType) y.Value))
                    .ToList();
            }

            foreach (var tit in selectedTemplates.Select(x => x.Value).Cast<TemplateInstanceType>())
            {
                if (!isNew && templates.Any(x => x.TemplateDescription.TemplateType == tit))
                    continue;
                templates.Add(new(tit));
            }
        }


        public async Task ModelDiscoveryAsync()
        {
            ManualResetEventSlim mrs = new ManualResetEventSlim(false);
            CancellationToken ctk = tokenSource.Token;
            var disco = new Discovery();
            _declareModel = await disco.DiscoverModelAsync(_selectedLog, templates, ctk);
        }

        public void AbortDiscovery()
        {
            tokenSource.Cancel();
        }

        public void SaveModel()
        {
            if (_declareModel is not null && !StateContainer.DeclareModels.Contains(_declareModel))
                StateContainer.DeclareModels.Add(_declareModel);
        }

        public bool IsSelectedTit(TemplateInstanceType tit)
        {
            return selectedTemplates?.Any(x => ((TemplateInstanceType) x.Value) == tit) ?? false;
        }

        public bool GetDefaultCheckValue(TemplateInstanceType tit)
        {
            switch (tit)
            {
                case TemplateInstanceType.Response:
                case TemplateInstanceType.Succession:
                case TemplateInstanceType.NotCoexistence:
                case TemplateInstanceType.Precedence:
                case TemplateInstanceType.Coexistence:
                    return true;
                default:
                    return false;
            }
        }


        public void Dispose()
        {
        }
    }
}