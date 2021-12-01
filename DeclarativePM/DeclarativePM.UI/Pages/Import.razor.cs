using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeclarativePM.Lib.Import;
using DeclarativePM.Lib.Models.LogModels;
using DeclarativePM.UI.Enums;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Import : ComponentBase
    {
        EventLog selectedLog;
        private MemoryStream _stream;
        string content;
        ImportedEventLog _imported;
        IMatFileUploadEntry file;
        bool uploadMode = false;
        string LogName;

        Dictionary<string, HeaderType> headersDict;
        HeaderType[] value2Items = Enum.GetValues(typeof(HeaderType)).Cast<HeaderType>().ToArray();


        protected override void OnInitialized()
        {
            selectedLog = StateContainer.EventLogs.FirstOrDefault();
        }

        public async Task UploadLog(IMatFileUploadEntry[] files)
        {
            file = files.FirstOrDefault();
            if (file is null)
                return;
            if (file.Type != "text/csv")
                return; //??
            try
            {
                _stream = new MemoryStream();

                await file.WriteToStreamAsync(_stream);
                _stream.Seek(0, SeekOrigin.Begin);
                GetUploadContent();
                _stream.Seek(0, SeekOrigin.Begin);
                _imported = ImportCsvLogs.LoadCsv(_stream);
                headersDict = _imported.Headers.ToDictionary(x => x, HeaderTypeSet);

                uploadMode = true;
            }
            catch
            {
                Console.WriteLine("Something went wrong");
                throw;
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private void ImportLog()
        {
            StateContainer.EventLogs.Add(_imported.BuildEventLog(LogName));
            _stream.Dispose();
            file = null;
            content = null;
            _imported = null;
            uploadMode = false;
            LogName = null;
        }

        private HeaderType HeaderTypeSet(string head)
        {
            if (_imported.Activity().Equals(head))
                return HeaderType.Activity;
            if (_imported.CaseId().Equals(head))
                return HeaderType.Case;
            return HeaderType.Resource;
        }

        public void GetUploadContent(int lines = 11)
        {
            var reader = new StreamReader(_stream);
            StringBuilder builder = new();
            while (!reader.EndOfStream && lines != 0)
            {
                var line = reader.ReadLine();
                builder.AppendLine(line);
                lines--;
            }

            content = builder.ToString();
        }

        public async Task Selection(EventLog log)
        {
            selectedLog = log;
            await InvokeAsync(StateHasChanged);
        }

        public void EnumChanged(HeaderType o, string key)
        {
            headersDict[key] = o;
        }

        public async Task ChooseTokens()
        {
            var act = headersDict.Where(x => x.Value == HeaderType.Activity);
            var caseId = headersDict.Where(x => x.Value == HeaderType.Case);
            if (!act.Any() || !caseId.Any())
            {
                await MatDialogService.AlertAsync("You need at least 1 activity and 1 case!");
                return;
            }

            var timetmpList = headersDict.Where(x => x.Value == HeaderType.Timestamp).Select(x => x.Key).ToList();
            var time = timetmpList.Any() ? timetmpList[0] : null;
            var resources = headersDict.Where(x => x.Value == HeaderType.Resource).Select(x => x.Key).ToArray();
            _imported.ChooseTokens(act.FirstOrDefault().Key, caseId.FirstOrDefault().Key, time, resources);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}