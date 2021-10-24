using System.Collections;
using System.Threading.Tasks;
using DeclarativePM.Lib.Models;
using DeclarativePM.UI.Enums;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace DeclarativePM.UI.Pages
{
    public partial class Create : ComponentBase
    {
        
        private CreateMethod method = CreateMethod.Undefined;
        private bool chooseMethod = true;
        private bool chooseLog = false;
        private EventLog SelectedLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val">0 for creation, 1 for creation from log</param>
        /// <returns></returns>
        public async Task ChooseMethod(CreateMethod val)
        {
            method = val;
            chooseMethod = false;
            chooseLog = true;
            await InvokeAsync(StateHasChanged);
        }
        
        public void Selection(EventLog log)
        {
            SelectedLog = log;
            StateHasChanged();
        }
        
        public string GetExpansionBackground(EventLog el)
        {
            return el.Equals(SelectedLog) ? "background: #ffd5ff" : "background: #f3f3f3";
        }
        
        public async Task ContinueCreate()
        {
            chooseLog = false;
            var result = await MatDialogService.AskAsync($"For {SelectedLog.Name} already existing" +
                                            $" DECLARE model was found, would you like to import it?", new[] {"YES", "NO", "CANCEL"});
            if (result == "CANCEL")
                return;
            if (result == "YES")
                return;
            if (result == "NO")
                return;
            await InvokeAsync(StateHasChanged);
        }
    }
}