using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Exceptions;

namespace DeclarativePM.Lib.Models
{
    public class ImportedEventLog
    {
        private string _activity;
        public string Activity
        {
            get
            {
                if (_activity is null)
                    throw new LogValueNotSetException(
                        "Activity property is not defined. You can define it by calling ChooseTokens method.");
                return Data[_activity];
            }
            private set
            {
                if (value is not null && _activity != value)
                {
                    _activity = value;
                }
            }
        }

        private string _caseId;
        public string CaseId
        {
            get
            {
                if (_caseId is null)
                    throw new LogValueNotSetException(
                        "CaseId property is not defined. You can define it by calling ChooseTokens method.");
                return Data[_caseId];
            }
            private set
            {
                if (value is not null && _caseId != value)
                {
                    _caseId = value;
                }
            }
        }

        private string _timeStampName;
        private DateTime? _timeStamp;
        public DateTime? TimeStamp
        {
            get
            {
                if (_timeStamp is null)
                    throw new LogValueNotSetException(
                        "TimeStamp property is not defined. You can define it by calling ChooseTokens method.");
                return _timeStamp;
            }
            private set
            {
                if (value is not null && _timeStamp != value)
                {
                    _timeStamp = value;
                }
            }
        }
        
        public Dictionary<string, string> Resources
        {
            get
            {
                var except = new[] {_activity, _caseId, _timeStampName};
                return Data.Where(d => !except.Contains(d.Key))
                    .ToDictionary(k => k.Key, v => v.Value);
            }
        }
        
        private Dictionary<string, string> Data { get; set; }

        public ImportedEventLog(Dictionary<string, string> data)
        {
            Data = data;
        }

        public IEnumerable<string> GetHeaders()
            => Data.Keys;
        

        public void ChooseTokens(string activity, string caseId, string timeStamp = null)
        {
            if (Data.ContainsKey(activity))
                Activity = activity;
            
            if (Data.ContainsKey(caseId))
                CaseId = caseId;
            
            if (timeStamp is null || !Data.ContainsKey(timeStamp)) return;
            if (DateTime.TryParse(Data[timeStamp], out var time))
            {
                TimeStamp = time;
                _timeStampName = timeStamp;
            }
        }
    }
}