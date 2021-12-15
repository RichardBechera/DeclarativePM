using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeclarativePM.Lib.Enums;
using DeclarativePM.Lib.Models.DeclareModels;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.Lib.Utils;
using DeclarativePM.UI.Data;
using DeclarativePM.UI.Utils;
using MatBlazor;

namespace DeclarativePM.UI.Pages
{
    public partial class Discover
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
            foreach (var pt in templates)
            {
                pt.TemplateInstances.Clear();
            }
            await InvokeAsync(StateHasChanged);
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
            CancellationToken ctk = tokenSource.Token;
            _declareModel = await Discovery.DiscoverModelAsync(_selectedLog, templates, ctk);
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


        public void Dispose()
        {
        }
    }
}