using System;
using System.Collections.Generic;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.UI.Data
{
    public class StateContainer
    {
        public List<EventLog> EventLogs { get; } = new();

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}